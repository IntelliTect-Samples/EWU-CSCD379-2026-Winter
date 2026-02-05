## Instructions

Use Guid's for all Id's. Create Entity Framework Core classes based on the following Tables. Always include navigation properties and navigation collections. Use DateTime for times/dates, where not specified.

### Stylists Table

- StylistId
- Name: String
- PhoneNumber: String
- ChairName: String
- WorkStartTime24H: decimal
- WorkEndTime24H: decimal

### Services Table

- ServiceId
- Name: String

### StylistServices Table

- SylistServiceId
- StylistId
- ServiceId
- DurationInMinutes: int
- Rate: decimal

### Appointments Table

- AppointmentId
- StylistId
- DateTime
- DurationInMinutes: int
- TotalPrice: decimal
- DatePaid
- CustomerName: String
- CustomerPhone: String

### AppointmentStylistServices Table

- AppointmentStylistServiceId
- AppointmentId
- StylistServiceId
