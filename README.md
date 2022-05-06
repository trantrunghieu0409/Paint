# Project Paint - Lập trình Windows - 19KTPM1

### Thành viên

|       Họ và tên      |   MSSV   |   Điểm  |   
|----------------------|:--------:|:-------:|
| Nguyễn Văn Tấn Phong | 19127646 |	   13   |
| Trần Trung Hiếu      | 19127641 |	   13   |
| Lê Quang Tấn Long    | 19127201 |	   13   |
| Trần Quốc Tuấn       | 19127650 |	   13   |

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

## Basic graphic object
- Line: controlled by two points, the starting point, and the endpoint
- Rectangle: controlled by two points, the left top point, and the right bottom point
- Ellipse: controlled by two points, the left top point, and the right bottom point

## Các chức năng đã làm được

** 1. Dynamically load all graphic objects that can be drawn from external DLL files **

** 2. The user can choose which object to draw **

** 3. The user can see the preview of the object they want to draw **

** 4. The user can finish the drawing preview and their change becomes permanent with previously drawn objects **

** 5. The list of drawn objects can be saved and loaded again for continuing later **

** 6. Save and load all drawn objects as an image in bmp/png/jpg format (rasterization). **

