Step 1. click clone .Clone the Repository: Use the git clone command followed by the repository URL.
Step 2: Open the Project in Visual Studio
Open Visual Studio: Launch Visual Studio on your computer. 
Click "Clone Repository"
paste the link
click clone
Step 3: Update Connection Strings
If your application requires a database connection, you might need to update the connection string in your appsettings.json file or wherever the connection string is defined. Make sure you have access to the database and the correct credentials.
Step 4: Build and Run the Application
Build the Project:
Click on Build > Build Solution to compile the project.
Run the Application:
Click on the green play button or press F5 to start the application. This will launch it in your default web browser.
Troubleshooting
If you encounter issues during cloning, building, or running the application, check for the following:
Ensure you have the correct version of the .NET SDK that the project targets.
Make sure all dependencies are correctly restored.
Check for any errors in the Error List window in Visual Studio.
Auto-Approval Integration:

Added a CheckAutoApprovalCriteria method to evaluate whether the claim meets predefined criteria.
Updated Claim_Sub to auto-approve claims if criteria are met.
File Handling:

Extracted file-saving logic to a helper method (SaveUploadedFile) for better modularity.
Improved Error Messages:

Used TempData for passing success or error messages to views.
General Cleanup:

Enhanced code readability by breaking complex logic into smaller, reusable methods.
