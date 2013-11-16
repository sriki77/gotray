Go Tray
=======

A Windows 8 application to serve as build radiator for [Thoughworks Go](http://www.thoughtworks.com/products/go-continuous-delivery). The application uses the CCTray feed to construct the dashboard.The application is intended to be hosted build radiator machines.

In addition, since the application works of CCTray feed, it can be used with any build system that can generate a CCTray Feed.

Snapshot of the dashboard is shown below.
![Dashboard Screenshot](https://raw.github.com/sriki77/gotray/master/screenshot.png)

#### Using the Application ####

+ Gotray is not hosted on windows store yet. Hence you either need to sideload it or build/run using Visual Studio 2012. 

+ Sideloading can done using the app package in the directory `GoTray/AppPackages/GoTray_1.0.0.4_AnyCPU_Test` 

+ After the application starts press `Windows Key + i` to bring out the configuration charm.

+ Enter the URL for the cctray xml. E.g `http://goserver:8153/go/cctray.xml`. Enter the username and password.

+ Once configured you should see the pipeline with their status being shown on the dashboard of the application. The data is refreshed every 5 mins.
