# API

## Endpoints
---
 
### GET /api/status/\{domain\}
---

> Fetches the current and previous status data of the specified domain.

#### Success Response
---

**Code**: `200 OK`

**Content example**:

```json
{
   "servicePageDomain":"github.com",
   "statusPageUrl":"https://status.github.com",
   "statusHistory":[
      {
         "id":146,
         "lastUpdateTime":"2022-07-13T14:03:25.4906558Z",
         "status":0,
         "ping":"00:00:00.7280000",
         "formatedUpdateTime":"332 milliseconds",
         "formatedPingTime":"728 milliseconds"
      },
      {
         "id":145,
         "lastUpdateTime":"2022-07-13T13:42:04.3931873",
         "status":0,
         "ping":"00:00:00.5990000",
         "formatedUpdateTime":"21 minutes",
         "formatedPingTime":"599 milliseconds"
      }
   ],
   "currentStatus":{
      "id":146,
      "lastUpdateTime":"2022-07-13T14:03:25.4906558Z",
      "status":0,
      "ping":"00:00:00.7280000",
      "formatedUpdateTime":"332 milliseconds",
      "formatedPingTime":"728 milliseconds"
   }
}
```

#### Error Response
---

**Condition**: If \{domain\} is not specified.

**Code**: `400 BAD REQUEST`

**Content example**: 
```
<placeholder>
```

---

**Condition** : If \{domain\} is currently not tracked.

**Code**: `404 NOT FOUND`

**Content example**: 
```
github.com is not tracked
```
