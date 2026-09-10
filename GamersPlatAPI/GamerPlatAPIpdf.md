## API Module

## Define API Functional :FR-115 Requirements

## Description

## .ةصنم  يمج ردو Backend ـو Frontend ـ  صوت ةمظ يب )5HE,E C'8FA 1:HJ

## Actors

- Player Application

- Owner Dashboard

- Admin Dashboard

- External Services

## API Requirements

- RESTful API Architecture

- JSON Data Format

- Secure Authentication

- Role-Based Access Control

- API Documentation (Swagger)

- Error Handling Standard

## Main Flow

- .Client sends API Request .1

- .API validates request .2

- .Business logic is executed .3

- .Database operation is performed .4

- .API returns response .5


## Acceptance Criteria

- .API Endpoint اه ةي ئاظو يمج

- .ايحلص سح ةيمحم APIs ـ يمج

## Authentication APIs :FR-116

## Description

.ةداصمو يمدختسم  ردإب ةصاخ اباسح APIs روت

## Actors

- Player

- Owner

- Admin

## Endpoints

## Register

## api/auth/register / POST

## Function:

- . يدجد اسح ءاشن

## Input:

- Name

- Email (optional)

- Password

- Phone

- Gender

- Date Of Birth

- Role


## api/auth/login / POST

Function:

- .Token اج وخد يجست

## Output:

- User ID

- Role

- JWT Token

## Verify Email

## api/auth/verify-email / POST

Function:

## Verify Phone (optional depend on pricing)

## api/auth/verify-PhoneNumber / POST

Function:

## Reset Password

## api/auth/reset-password / POST


Function:

- .رورم ةم ييعتك

## Change Password

## api/auth/change-password / PUT

Function:

- .رورم ةم رييغتك

## Acceptance Criteria

- امك

- .JWT Authentication دختس

- .ةساسح endpoints ـ ةيامح

## Player APIs :FR-117

## Description

.  ئاظوب ةصاخ 5DA APIs ريوت

## Actors

- Player

## Get Profile

## api/player/profile / GET

Returns:


- Personal Information

- Points Balance

- Booking Summary

- statistics

## Update Profile

## api/player/profile / PUT

## Updates:

- Name

- Phone

- Image

- Preferences

## Browse Centers

## api/centers / GET

## Returns:

- Center List

- Rating

- Location

- Services

## View Center Details

## id}{/api/centers / GET

## Returns:

- Center Information

- Devices


- Offers

- Tournaments

## Create Booking

## api/bookings / POST

## Input:

- Center ID

- Device ID Or Service ID

- Date

- Time

- Duration

## Cancel Booking

## cancel/ id} {/api/bookings / PUT

## Postponing Booking

## cancel/ id} {/api/bookings / PUT

## Booking History

api/player/bookings / GET

## Submit Rating

api/reviews / POST


## Acceptance Criteria

- .7B: H*'F'J(A @H5HA 4J7*3J 5DA

## Owner APIs :FR-118

## Description

## .اع زكرم ردإب ةصاخ APIs ريوت

## Actors

- Gaming Center Owner

## Create Center

## api/owner/centers / POST

## Input:

- Center Information

- Location

- Images

## Update Center

## id}{/api/owner/centers / PUT


## Get Owner Centers

## api/owner/centers / GET

## Manage Resources

## Add Resources (Every thing can be booked)

## api/owner/Resources / POST

## Update Resources

## id}{/ Resources /api/owner / PUT

## Manage Bookings

## api/owner/bookings / GET

## Returns:

- Player

- Device

- Date

- Status

## Update Booking Status

## status/ id} {/api/owner/bookings / PUT


## Manage Offers

## id}{/api/owner/offers/ DELETE id}{/api/owner/offers/ PUT api/owner/offers / POST

## Manage Tournaments

## api/owner/tournaments / POST

## Dashboard

## api/owner/dashboard / GET

## Returns:

- Bookings

- Revenue

- Devices

- Statistics

## Acceptance Criteria

- .طق زكرم  رد يطتسي انايب Owner


## Admin APIs :FR-119

## Description

.اظن ردإب ةصاخ APIs ريوت

## Actors

- Admin

## Users Management

## Get Users

## api/admin/users / GET

## block/ id} {/api/admin/users / PUT Block User

## Unblock User

## unblock/ id} {/api/admin/users / PUT

## Centers Management

## Get Centers

## api/admin/centers / GET


## Approve Center

## approve/ id} {/api/admin/centers / PUT

## Reject Center

## reject/ id} {/api/admin/centers / PUT

## System Management

## APIs

- Cities

- Countries

- Device Types

- Categories

- Offers

- Tournaments

## Analytics

## api/admin/analytics / GET

## Returns:

- Users Count

- Centers Count

- Bookings

- Revenue

## Acceptance Criteria

- .ةيمحم Admin APIs يمج


- . )J 'JBE9A @J,3* 1/T

## Booking APIs :FR-120

## Description

API. رب جح 2 ‐'JBE5 4JE,

## Endpoints

## Create Booking

POST

/api/bookings

## Get Booking Details

GET

id}{//api/bookings

## Cancel Booking

PUT

cancel/}id{//api/bookings

## Booking Availability

GET

/api/bookings/check-availability


## Validation

- Device Availability

- Booking Conflict

- User Authorization

## Tournament APIs :FR-121

## Description

## Endpoints

## Get Tournaments

GET

/api/tournaments

## Tournament Details

GET

id}{//api/tournaments

## Join Tournament

POST

join/}id{//api/tournaments


## Leave Tournament

DELETE

leave/}id{//api/tournaments

## Results

GET

results/}id{//api/tournaments

## Notification APIs :FR-122

## Description

## Endpoints

## Get Notifications

GET

/api/notifications

## Mark As Read

PUT

read/}id{//api/notifications


## Send Notification

POST

/api/admin/notifications

## Acceptance Criteria

- .7B: H*1'94 I C/.*3EA ري

## Reports APIs :FR-123

## Description

.ايئاصحو ريراقت APIs ريوت

## Endpoints

## Player Reports

GET

## /api/player/reports

## Owner Reports

GET

/api/owner/reports


## Revenue Reports

GET

## /api/reports/revenue

## System Analytics

GET

## /api/admin/reports

## API Validation :FR-124

## Description

## .اذيفنت  API  يمج ةحصم اظن قحتي ابط

## Validation Requirements

- Required Fields Validation

- Data Type Validation

- Authorization Validation

- Business Rules Validation

- Duplicate Data Validation

## Examples

- .روتم ر زاهجي زجح نم

- فنب

- .كمي  كرم ز يدعت م Owner نم


## API Error Responses :FR-125

## Description

```
APIs. ـ يمج دحوم ءاطخ اباجتس اظن ديعي
```

## Error Format

```
Example:
{
,false :" "success
, Device not available " " :"message"
BOOKING_001"" :"errorCode"
}
```

## HTTP Status Codes

## Code Usage

| 200 | Success |
| --- | --- |
| 201 | Created |
| 400 Bad Request |   |
| 401 Unauthorized |   |
| 403 Forbidden |   |
| 404 Not Found |   |
| 409 Conflict |   |
| 500 Server Error |   |

## Business Rules

- .Authentication ات اسححي API  ك :BR-077

- .API ـ وصو ددحت دختسم ايحلص :BR-078

- .اهن قحتم  جي Client م ةمد انايب يمج :BR-079

- .Responses  ساسح ة انايب شك   تي :BR-080


- .APIs يمج يثوت  جي :BR-081

## Input Requirements

- Request Headers

- Authentication Token

- Request Body

- Query Parameters

- Route Parameters

## Output Requirements

## :ديعي   جي API

- Status Code

- Success/Failure

- Data

- Message

- )ةجاح دن( Error Details

## Error Handling

- Invalid Token

- Unauthorized Request

- Validation Error

- Resource Not Found

- Database Error

- Server Error


## Module 15صخلم

- FR-115 Define API Functional Requirements

- FR-116 Authentication APIs

- FR-117 Player APIs

- FR-118 Owner APIs

- FR-119 Admin APIs

- FR-120 Booking APIs

- FR-121 Tournament APIs

- FR-122 Notification APIs

- FR-123 Reports APIs

- FR-124 API Validation

- FR-125 API Error Responses

بط

.سوت ةباو ةنمآو ةمظ ةقيرطبنم

Backend ـ م Frontend ـ افتيس

 ددحيويك

«GamersPlat
