# Paint


## Hướng dẫn chạy lần đầu:

1. Clean solution -> Build/Rebuild solution
2. Vào thư mục \Paint\LineEntity\bin\Debug\net6.0-windows -> Copy file **LineEntity.dll** -> Paste sang thư mục \Paint\Paint\bin\Debug\net6.0-windows
***Nếu thấy trong thư mục \Paint\Paint\bin\Debug\net6.0-windows có file LineEntity.dll rồi thì xóa file đó đi và thực hiện copy như trên***
3. Làm tương tự với RectangeEntity


## Hướng dẫn cách mở rộng Loại vẽ:
1. Tạo project kiểu **"WPF Class Library"**, đặt tên 2 file theo định dạng <Loại hình vẽ>Entity.cs và <Loại hình vẽ>Painter.cs ***(có thể tham khảo cách làm với LineEntity)***
2. Nhấp chuột phải vào Dependencies -> chọn Add Project Preferences... -> chọn IContract 
Lưu ý:
Nếu sử dụng **Point** thì cần include System.Windows, ***không include System.Drawings nhé*** 
3. Mỗi lần code tạo mới hoặc thay đổi liên quan đến hình vẽ, rebuild lại project( của hình vẽ đó ) và copy file .dll vào trong Paint ( giống như chạy lần đầu )