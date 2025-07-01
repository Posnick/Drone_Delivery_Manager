This C#.NET project follows a three-layer architecture.

The presentation layer is implemented using WPF.
The business logic layer handles all core operations.
The data access layer (DAL) is implemented in two versions. The first version stores data in memory using lists, meaning all data is lost when the application closes. The second version stores data in XML files, allowing data to persist between sessions.

The application provides two user interfaces: one for customers and one for administrators.

In the customer interface, users can view their parcels and send new parcels.

In the admin interface, users can view lists of drones, base stations, customers, and parcels. From each list, a specific item can be selected to view detailed information.

The project uses the Factory design pattern to manage the two DAL implementations. To switch between them, edit the dal-config.xml file and update the third line to match the desired implementation.

A drone simulator is included to simulate the life cycle of a drone. The simulator uses a BackgroundWorker. To activate the simulator, log in as admin, go to the drone list, double-click a drone, and click the "Simulator" button.

When using the XML-based DAL, it is recommended to press the "Reset" button on the main window and restart the project before continuing.

Login credentials are as follows:
For customers, usernames are customer0, customer1, and so on. The password is the same as the username.
For admin, both the username and password are "admin".
