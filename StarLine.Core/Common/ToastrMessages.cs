namespace AfternoonLaugh.Infrastructure.Helper
{

    //Reference Link : https://github.com/nabinked/NToastNotify
    public class ToastrMessages
    {
        public static string Add = "Inserted successfully !"; // This is for Record Add Notifications
        public static string Update = "Updated successfully !"; // This is for Record Update Notifications
        public static string Delete = "Deleted successfully !"; // This is for Record Delete Notifications
        public static string Error = "Error while processing request !"; // This is for Error while CRUD operations
        public static string Save = "Saved successfully !";
        public static string GetMsg(string ModuleName, string Msg)
        {
            //ModuleName = "Record ";
            if (string.IsNullOrEmpty(ModuleName))
                return $"{Msg}";
            else
                return $"{ModuleName} {Msg}";
        }
    }
    public static class ToastrModules
    {
        public static string Branch = "Branch: ";
        public static string Department = "Department: ";
        public static string Designation = "Designation: ";
        public static string Employee = "Employee: ";
        public static string Holiday = "Holiday: ";
        public static string LeaveType = "Leave Type: ";
        public static string Notice = "Notice :";
        public static string Role = "Role :";
        public static string Shift = "Shift & Shift Group :";
    }
}
