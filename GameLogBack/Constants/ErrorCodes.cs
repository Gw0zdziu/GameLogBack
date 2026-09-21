namespace GameLogBack.Constants;

public static class ErrorCodes
{
    public static class Auth
    {
        public const string IncorrectDataOfLogin = "auth.incorrect-data-of-login";
        public const string ExpiredRefreshToken = "auth.expired-refresh-token";
    }
    
    public static class User
    {
        public const string UserNotFound = "user.not-found";
        public const string UsernameAlreadyTaken = "user.username-already-taken";
        public const string EmailAlreadyTaken = "user.email-already-taken";
        public const string VerificationCodeNotFound = "user.verification-code-not-found";
        public const string ExpiredVerificationCode = "user.expired-verification-code";
        public const string IncorrectVerificationCode = "user.incorrect-verification-code";
        public const string PasswordsDoNotMatch = "user.passwords-do-not-match";
        public const string RecoveryCodeHasExpired = "user.recovery-code-has-expired";
        public const string RecoveryCodeIsAlreadyUsed = "user.recovery-code-is-already-used";
    }

    public static class Category
    {
        public const string CategoryNotFound = "category.not-found";
        public const string ExistGameWithThisCategory = "category.exist-game-with-this-category";
        public const string CategoryWithThisNameAlreadyExists = "category.category-with-this-name-already-exists";
    }

    public static class Game
    {
        public const string GameNotFound = "game.not-found";
        public const string GameWithThisNameAlreadyExists = "game.game-with-this-name-already-exists";
        
    }
    
    public static class Utils
    {
        public const string UtilsTokenExpired = "utils.token-expired";
        public const string UtilsTokenIsNull  = "utils.token-is-null";
        public const string UtilsTokenHasInvalidFormat = "utils.token-has-invalid-format";
        public const string UtilsTokenIsInvalid = "utils.token-is-invalid";
    }
    
    public static class Internal
    {
        public const string InternalServerError = "internal.server-error";
    }
    
}