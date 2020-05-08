# WCCLS Mobile Project

This is a project to bring the Washington County Cooperative Library Services web services into mobile apps. Mobile apps are generally friendlier to use and have the added benefits of stored credentials and push notifications. The tech stack for this project is as follows.

- .NET Core Backend - An API that sits between the mobile app and the library website. If something breaks, it can be instantly redeployed with no changes from the apps perspective.

- Xamarin.Forms Frontend - Because this app will be lightweight, Xamrin.Forms will be perfect for quickly refining the UI. I will attempt to still implment a lot of the features people love such as push notifications, bioauthentication, and light mode/dark mode.
