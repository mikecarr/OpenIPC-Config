# OpenIPC Config
A multiplatform application


Mac
![img.png](images/main_app.png)

Windows
![img.png](images/main_app_win.png)




## Publish
* Mac
    ```bash
    dotnet publish -c Release -r osx-arm64 --self-contained
    ```
* Windows
    ```bash
    dotnet publish -c Release -r win-x64 --self-contained
    ```
  
* Linux x64
    ```bash
    dotnet publish -c Release -r linux-x64 --self-containeddotnet publish -c Release -r linux-x64 --self-contained
    ```
  
* For ARM-based Linux distributions (e.g., Raspberry Pi, Radxa?), use:
    ```bash
    dotnet publish -c Release -r linux-arm --self-contained
    ```

* One command:
  ```bash
    dotnet publish -c Release -r osx-arm64 --self-contained -o ./publish/osx-arm64
    dotnet publish -c Release -r linux-x64 --self-contained -o ./publish/linux-x64
    dotnet publish -c Release -r win-x64 --self-contained -o ./publish/win-x64
  ```
