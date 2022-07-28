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
         "status":"Up",
         "lastUpdateTime":"2022-07-13T14:03:25.4906558Z",
         "responseTime":"00:00:00.7280000",
         "formatedUpdateTime":"332 milliseconds",
         "formatedResponseTimeTime":"728 milliseconds"
      },
      {
         "id":145,
         "status":"Up",
         "lastUpdateTime":"2022-07-13T13:42:04.3931873",
         "responseTime":"00:00:00.5990000",
         "formatedUpdateTime":"21 minutes",
         "formatedResponseTimeTime":"599 milliseconds"
      }
   ],
   "currentStatus":{
      "id":146,
      "status":"Up",
      "lastUpdateTime":"2022-07-13T14:03:25.4906558Z",
      "responseTime":"00:00:00.7280000",
      "formatedUpdateTime":"332 milliseconds",
      "formatedResponseTimeTime":"728 milliseconds"
   }
}
```

#### Error Response
---

**Condition**: If \{domain\} is not specified.

**Code**: `400 BAD REQUEST`

**Content example**: 
```
{
   "type":"https://tools.ietf.org/html/rfc7231#section-6.5.1",
   "title":"One or more validation errors occurred.",
   "status":400,
   "traceId":"00-3ea5a8cdfe88cc7202c3398b9102f363-e1328277740c25c3-00",
   "errors":{
      "domain":[
         "The domain field is required."
      ]
   }
}
```

---

**Condition** : If \{domain\} is currently not tracked.

**Code**: `404 NOT FOUND`

**Content example**: 
```
{
   "type":"https://tools.ietf.org/html/rfc7231#section-6.5.4",
   "title":"Not Found",
   "status":404,
   "detail":"github.com is not tracked",
   "traceId":"00-b8b369d6b3bfc903470a05dd9a0945fb-23206b8efe366a0d-00"
}
```

---

**Condition** : If rate limit exceeded.

**Code**: `429 TOO MANY REQUESTS`

**Content example**: 
```
{
   "type":"https://tools.ietf.org/html/rfc6585#section-4",
   "title":"Too Many Requests",
   "status":429,
   "detail":"API calls quota exceeded! maximum admitted 5 per 1m. Retry again in 57s."
}
```