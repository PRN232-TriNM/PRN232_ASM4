# SOAP API Postman Guide - Station Service

## Tổng quan

SOAP API Service cho quản lý EV Charging Stations. Service sử dụng SOAP 1.1 protocol.

**Base URL:**
- HTTP: `http://localhost:5046`
- HTTPS: `https://localhost:7176`

**SOAP Endpoint:** `/StationService.asmx`

**SOAP Version:** SOAP 1.1

**Content-Type:** `text/xml; charset=utf-8`

---

## Cấu hình Postman

### 1. Tạo Request mới
1. Mở Postman
2. Tạo request mới (New → HTTP Request)
3. Chọn method: **POST**
4. URL: `http://localhost:5046/StationService.asmx`

### 2. Headers
Thêm các headers sau:

```
Content-Type: text/xml; charset=utf-8
SOAPAction: [Tên operation] (xem chi tiết từng operation)
```

### 3. Body
- Chọn tab **Body**
- Chọn **raw**
- Chọn format **XML**
- Paste SOAP envelope vào

---

## Các Operations

### 1. GetAllAsync - Lấy tất cả Stations

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/GetAllAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetAllAsync xmlns="http://tempuri.org/">
    </GetAllAsync>
  </soap:Body>
</soap:Envelope>
```

**Response Example:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetAllAsyncResponse xmlns="http://tempuri.org/">
      <GetAllAsyncResult>
        <Station>
          <StationId>1</StationId>
          <StationCode>ST001</StationCode>
          <StationName>Station 1</StationName>
          <Address>123 Main St</Address>
          <City>Ho Chi Minh</City>
          <Province>Ho Chi Minh</Province>
          <Latitude>10.762622</Latitude>
          <Longitude>106.660172</Longitude>
          <Capacity>10</Capacity>
          <CurrentAvailable>8</CurrentAvailable>
          <Owner>EVCS Company</Owner>
          <ContactPhone>0123456789</ContactPhone>
          <ContactEmail>contact@evcs.com</ContactEmail>
          <Description>Main charging station</Description>
          <CreatedDate>2024-01-01T00:00:00</CreatedDate>
          <ModifiedDate>2024-01-02T00:00:00</ModifiedDate>
          <IsActive>true</IsActive>
        </Station>
      </GetAllAsyncResult>
    </GetAllAsyncResponse>
  </soap:Body>
</soap:Envelope>
```

---

### 2. GetAsync - Lấy Station theo ID

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/GetAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetAsync xmlns="http://tempuri.org/">
      <id>1</id>
    </GetAsync>
  </soap:Body>
</soap:Envelope>
```

**Response Example:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetAsyncResponse xmlns="http://tempuri.org/">
      <GetAsyncResult>
        <StationId>1</StationId>
        <StationCode>ST001</StationCode>
        <StationName>Station 1</StationName>
        <Address>123 Main St</Address>
        <City>Ho Chi Minh</City>
        <Province>Ho Chi Minh</Province>
        <Latitude>10.762622</Latitude>
        <Longitude>106.660172</Longitude>
        <Capacity>10</Capacity>
        <CurrentAvailable>8</CurrentAvailable>
        <Owner>EVCS Company</Owner>
        <ContactPhone>0123456789</ContactPhone>
        <ContactEmail>contact@evcs.com</ContactEmail>
        <Description>Main charging station</Description>
        <CreatedDate>2024-01-01T00:00:00</CreatedDate>
        <ModifiedDate>2024-01-02T00:00:00</ModifiedDate>
        <IsActive>true</IsActive>
      </GetAsyncResult>
    </GetAsyncResponse>
  </soap:Body>
</soap:Envelope>
```

---

### 3. SearchStationsAsync - Tìm kiếm Stations

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/SearchStationsAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <SearchStationsAsync xmlns="http://tempuri.org/">
      <searchTerm>ST001</searchTerm>
    </SearchStationsAsync>
  </soap:Body>
</soap:Envelope>
```

**Parameters:**
- `searchTerm` (string): Từ khóa tìm kiếm (StationCode, StationName, hoặc Address)

---

### 4. GetAvailableStationsAsync - Lấy Stations có sẵn

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/GetAvailableStationsAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <GetAvailableStationsAsync xmlns="http://tempuri.org/">
    </GetAvailableStationsAsync>
  </soap:Body>
</soap:Envelope>
```

---

### 5. CreateStationAsync - Tạo Station mới

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/CreateStationAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <CreateStationAsync xmlns="http://tempuri.org/">
      <station>
        <StationCode>ST002</StationCode>
        <StationName>New Station</StationName>
        <Address>456 Second St</Address>
        <City>Hanoi</City>
        <Province>Hanoi</Province>
        <Latitude>21.0285</Latitude>
        <Longitude>105.8542</Longitude>
        <Capacity>20</Capacity>
        <CurrentAvailable>20</CurrentAvailable>
        <Owner>EVCS Company</Owner>
        <ContactPhone>0987654321</ContactPhone>
        <ContactEmail>station2@evcs.com</ContactEmail>
        <Description>New charging station in Hanoi</Description>
        <IsActive>true</IsActive>
      </station>
    </CreateStationAsync>
  </soap:Body>
</soap:Envelope>
```

**Required Fields:**
- `StationCode` (string, max 50): Mã station (phải unique)
- `StationName` (string, max 100): Tên station
- `Address` (string, max 200): Địa chỉ
- `Owner` (string, max 150): Chủ sở hữu
- `Capacity` (int, 1-1000): Số lượng chỗ sạc

**Optional Fields:**
- `City` (string, max 100)
- `Province` (string, max 100)
- `Latitude` (decimal, -90 to 90)
- `Longitude` (decimal, -180 to 180)
- `CurrentAvailable` (int, 0-1000): Mặc định = Capacity nếu không set
- `ContactPhone` (string, max 20)
- `ContactEmail` (string, max 150, email format)
- `Description` (string, max 500)
- `IsActive` (bool): Mặc định = true khi tạo mới

**Response:**
Trả về `StationId` (int) của station vừa tạo.

---

### 6. UpdateStationAsync - Cập nhật Station

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/UpdateStationAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <UpdateStationAsync xmlns="http://tempuri.org/">
      <station>
        <StationId>1</StationId>
        <StationCode>ST001</StationCode>
        <StationName>Updated Station Name</StationName>
        <Address>123 Main St Updated</Address>
        <City>Ho Chi Minh</City>
        <Province>Ho Chi Minh</Province>
        <Latitude>10.762622</Latitude>
        <Longitude>106.660172</Longitude>
        <Capacity>15</Capacity>
        <CurrentAvailable>12</CurrentAvailable>
        <Owner>EVCS Company</Owner>
        <ContactPhone>0123456789</ContactPhone>
        <ContactEmail>contact@evcs.com</ContactEmail>
        <Description>Updated description</Description>
        <IsActive>true</IsActive>
      </station>
    </UpdateStationAsync>
  </soap:Body>
</soap:Envelope>
```

**Note:** Phải include `StationId` trong request.

**Response:**
Trả về `StationId` (int) của station đã cập nhật.

---

### 7. DeleteStationAsync - Xóa Station (Soft Delete)

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/DeleteStationAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <DeleteStationAsync xmlns="http://tempuri.org/">
      <id>1</id>
    </DeleteStationAsync>
  </soap:Body>
</soap:Envelope>
```

**Response:**
- Trả về `1` nếu xóa thành công
- Trả về `0` nếu không tìm thấy station
- Throw FaultException nếu station có chargers

**Note:** Đây là soft delete (set IsActive = false), không xóa khỏi database.

---

### 8. ActivateStationAsync - Kích hoạt Station

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/ActivateStationAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <ActivateStationAsync xmlns="http://tempuri.org/">
      <id>1</id>
    </ActivateStationAsync>
  </soap:Body>
</soap:Envelope>
```

**Response:**
- `true` nếu kích hoạt thành công
- `false` nếu không tìm thấy station

---

### 9. UpdateAvailabilityAsync - Cập nhật số lượng available

**SOAPAction:** `http://tempuri.org/IStationSOAPServices/UpdateAvailabilityAsync`

**Request Body:**
```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <UpdateAvailabilityAsync xmlns="http://tempuri.org/">
      <stationId>1</stationId>
      <newAvailable>5</newAvailable>
    </UpdateAvailabilityAsync>
  </soap:Body>
</soap:Envelope>
```

**Parameters:**
- `stationId` (int): ID của station
- `newAvailable` (int): Số lượng available mới (phải >= 0 và <= Capacity)

**Response:**
- `true` nếu cập nhật thành công
- `false` nếu không tìm thấy station hoặc giá trị không hợp lệ

---

## Error Handling

Khi có lỗi, SOAP service sẽ trả về FaultException:

```xml
<?xml version="1.0" encoding="utf-8"?>
<soap:Envelope xmlns:soap="http://schemas.xmlsoap.org/soap/envelope/">
  <soap:Body>
    <soap:Fault>
      <faultcode>soap:Server</faultcode>
      <faultstring>Error message here</faultstring>
    </soap:Fault>
  </soap:Body>
</soap:Envelope>
```

**Common Errors:**
- `Validation error: [message]` - Dữ liệu không hợp lệ
- `Business rule violation: [message]` - Vi phạm business rules
- `Error [operation]: [message]` - Lỗi chung

---

## Postman Collection Setup

### Tạo Collection trong Postman:

1. **New Collection** → Đặt tên "EVCS SOAP API"
2. Tạo các requests cho từng operation
3. Lưu các SOAP envelopes vào từng request

### Pre-request Script (Optional):

Để tự động set SOAPAction header:

```javascript
// Set SOAPAction header based on operation name
const operationName = pm.request.name;
const soapAction = `http://tempuri.org/IStationSOAPServices/${operationName}`;
pm.request.headers.add({
    key: 'SOAPAction',
    value: soapAction
});
```

---

## Testing Tips

1. **Kiểm tra Service đang chạy:**
   - Mở browser: `http://localhost:5046/StationService.asmx`
   - Nếu thấy SOAP service description thì service đang chạy

2. **Test từng operation:**
   - Bắt đầu với `GetAllAsync` để lấy danh sách stations
   - Dùng `GetAsync` với ID từ GetAllAsync
   - Test Create → Update → Delete

3. **Validate Response:**
   - Kiểm tra HTTP Status Code (200 = success)
   - Kiểm tra SOAP envelope structure
   - Kiểm tra dữ liệu trong response

4. **Debug:**
   - Nếu gặp lỗi, kiểm tra:
     - URL đúng chưa
     - Headers đúng chưa (Content-Type, SOAPAction)
     - SOAP envelope format đúng chưa
     - Service đang chạy chưa

---

## WSDL URL

Để xem WSDL definition:
- `http://localhost:5046/StationService.asmx?wsdl`

---

## Notes

- Tất cả dates sử dụng format ISO 8601: `yyyy-MM-ddTHH:mm:ss`
- Boolean values: `true` hoặc `false` (lowercase)
- Decimal values: Sử dụng dấu chấm (.) cho decimal separator
- Nullable fields có thể bỏ qua trong request
- StationCode phải unique trong hệ thống

---

## Contact

Nếu có vấn đề, kiểm tra:
1. Service logs trong console
2. Database connection
3. SOAP envelope format
4. Network connectivity

