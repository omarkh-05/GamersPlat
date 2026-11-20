# Notifications API and Event Logic

## Overview

Notifications are persisted in the existing `Notifications` table and retrieved through authenticated API requests. Newly saved notifications are also delivered in real time through SignalR when the recipient is connected. The database remains the source of truth; offline recipients can retrieve saved notifications through the APIs below.

All endpoints require the existing JWT bearer authentication. The API derives the recipient from the authenticated `NameIdentifier` claim; clients must not send a `UserId` to select whose notifications are returned or changed.

## API endpoints

Base route: `/api/notifications`

| Method | Route | Description | Success |
| --- | --- | --- | --- |
| `GET` | `/api/notifications` | List the authenticated user's notifications, newest first | `200` with an array |
| `GET` | `/api/notifications/unread-count` | Get unread notification count for the authenticated user | `200` with `{ "count": number }` |
| `PATCH` | `/api/notifications/{id}/read` | Mark one of the authenticated user's notifications as read | `204`; `404` if it does not belong to the user or does not exist |
| `PUT` | `/api/notifications/read/{id}` | Backward-compatible route for marking one notification as read | `204`; `404` if it does not belong to the user or does not exist |
| `PATCH` | `/api/notifications/read-all` | Mark all of the authenticated user's unread notifications as read | `204` |

Unauthenticated requests receive `401`. Notification responses do not include recipient `UserId` or entity navigation data.

### Notification response shape

```json
{
  "notificationId": 42,
  "centerId": 8,
  "title": "Booking created",
  "message": "Player Alex created booking #105 at North Center for 2026-05-01, 18:00-19:00. Total: $25.00.",
  "type": "Booking",
  "status": "Unread",
  "referenceType": "BookingCreated:105",
  "readAt": null,
  "createdAt": "2026-04-21T16:20:00Z"
}
```

`centerId` may be `null` for notifications without a center. New notifications have `status: "Unread"`, `readAt: null`, and a UTC `createdAt`. Marking a notification read changes its status to `Read` and sets `readAt` to UTC time. Use `notificationId` for the mark-read path; do not treat `referenceType` as an API resource identifier.

## Event categories and recipients

`type` is a stable category (`Booking` or `Center`). `referenceType` identifies the event and related booking/center; it is suitable for frontend navigation/context, not authorization.

| Event | Type | Reference type | Recipients |
| --- | --- | --- | --- |
| Player creates a booking | `Booking` | `BookingCreated:{bookingId}` | Center owner and active staff in that center with `AcceptRejectBooking` permission |
| Player cancels a booking | `Booking` | `BookingCancelled:{bookingId}` | Center owner and active staff in that center with `AcceptRejectBooking` permission |
| Player updates a booking | `Booking` | `BookingUpdated:{bookingId}` | Center owner and active staff in that center with `AcceptRejectBooking` permission |
| Owner/authorized staff accepts a booking | `Booking` | `BookingAccepted:{bookingId}` | Player who created the booking |
| Owner/authorized staff rejects a booking | `Booking` | `BookingRejected:{bookingId}` | Player who created the booking |
| Staff/owner stops a booking and invoice generation succeeds | `Booking` | `BookingCompleted:{bookingId}` | Center owner; message includes the generated invoice number |
| Owner creates a center | `Center` | `CenterCreated:{centerId}` | Users assigned the `admin` role only |
| Admin accepts a center | `Center` | `CenterAccepted:{centerId}` | Center owner |
| Admin rejects a center | `Center` | `CenterRejected:{centerId}` | Center owner |
| Center is activated | `Center` | `CenterActivated:{centerId}` | Center owner |
| Center is deactivated | `Center` | `CenterDeactivated:{centerId}` | Center owner |

Booking-management staff are selected from the center's existing `Staff` relationship and must be active (`IsActive`, active staff status, active user) and have the `AcceptRejectBooking` permission bit. Other staff and unrelated users are not notified. Admin recipient selection uses the existing user-role relationship and the `admin` role.

Notification writes are deduplicated per recipient, category, and event reference using the existing columns. A notification persistence error is logged and does not change the result of the booking, invoice, or center operation.

## Suggested frontend behavior

1. Fetch `GET /api/notifications` for the current authenticated session and render items in the returned order.
2. Fetch `GET /api/notifications/unread-count` for a badge/count. Refresh it after marking a notification read or after an explicit list refresh.
3. For an unread item, call `PATCH /api/notifications/{notificationId}/read`; on `204`, update its local status to `Read` and set its local read time, or refetch the list.
4. Use `PATCH /api/notifications/read-all` for a “mark all read” action and then refresh the list/count.
5. Use `type`, `referenceType`, and `centerId` to choose an in-app destination if that destination exists; message text is display-only content.

## SignalR real-time delivery

Connect to `/notificationHub` on the API host using the current JWT access token. The hub requires authentication and associates each connection with the user ID in the token's `NameIdentifier` claim; clients do not provide a recipient ID or join user groups themselves.

With the JavaScript SignalR client (`@microsoft/signalr`):

```javascript
const connection = new signalR.HubConnectionBuilder()
  .withUrl(`${apiBaseUrl}/notificationHub`, {
	accessTokenFactory: () => accessToken
  })
  .withAutomaticReconnect()
  .build();

connection.on("ReceiveNotification", notification => {
  // notification has the same DTO shape as an item from GET /api/notifications
});

await connection.start();
```

The access token is sent as the `access_token` query parameter for the hub endpoint and is validated by the existing JWT bearer configuration. Handle `ReceiveNotification` by adding the returned notification DTO to the current user's view and refreshing any unread count as needed. Multiple connections for the same user receive the event. If the user is offline or delivery fails, the notification remains persisted and can be retrieved through `GET /api/notifications` after reconnecting; there is no replay buffer or pagination parameter.

## Database and deployment

No table, column, entity, or migration changes are required. The implementation uses the existing `Notifications` schema and database-first `Notification` entity.
