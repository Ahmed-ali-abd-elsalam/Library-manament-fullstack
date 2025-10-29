namespace Application.Results
{
    public class Errors
    {
        public static readonly Error WrongPassword = new Error("Enter your password");
        public static Error DoesntExist(string entity) => new Error($"Requested {entity} doesn't exist");
        public static readonly Error NotAvailable = new Error("requested entity not Available");
        public static readonly Error EmailNotConfirmed = new Error("Email is Not Confirmed");
        public static readonly Error RefreshToken = new Error("Expired Refresh Token please login again");
        public static readonly Error InvalidToken = new Error("Invalid Token please use the correct one");
        public static readonly Error EmailTaken = new Error("This is Email is Taken");
        public static readonly Error PasswordNotSecure = new Error("Password not Secure it should contain one uppercase,one lower case ,one digit , one special character and 12 characters long");
        public static readonly Error DeletionFailed = new Error("failed To Delete this entity");
        public static readonly Error DuplicateEntry = new Error("an entity already exists with this values");
        public static readonly Error RepeatedOperation = new Error("that operation is already completed");
        public static readonly Error DoesntBelong = new Error("that enity doesn't belong to the logged in user");
        public static readonly Error InvalidInputs = new Error("invalid input this input is not accepted");
        public static readonly Error UnAuthorizedOperation = new Error("Current User cant do this operation");
        public static readonly Error CantApprove = new Error("Cant Approve borrowing for this book no items in stock");


    }
}
