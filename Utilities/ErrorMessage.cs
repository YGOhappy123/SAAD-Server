using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace milktea_server.Utilities
{
    public static class ErrorMessage
    {
        public const string USERNAME_EXISTED = "USERNAME_EXISTED";
        public const string EMAIL_EXISTED = "EMAIL_EXISTED";
        public const string USER_NOT_FOUND = "USER_NOT_FOUND";
        public const string GOOGLE_AUTH_FAILED = "GOOGLE_AUTH_FAILED";
        public const string ORDER_NOT_FOUND = "ORDER_NOT_FOUND";
        public const string CART_ITEM_NOT_FOUND = "CART_ITEM_NOT_FOUND";
        public const string INVALID_CREDENTIALS = "INVALID_CREDENTIALS";
        public const string INCORRECT_USERNAME_OR_PASSWORD = "INCORRECT_USERNAME_OR_PASSWORD";
        public const string DATA_VALIDATION_FAILED = "DATA_VALIDATION_FAILED";
        public const string UPLOAD_IMAGE_FAILED = "UPLOAD_IMAGE_FAILED";
        public const string DELETE_IMAGE_FAILED = "DELETE_IMAGE_FAILED";
        public const string NO_PERMISSION = "NO_PERMISSION";
        public const string CREATE_ORDER_FAILED = "CREATE_ORDER_FAILED";
        public const string PRODUCT_NOT_FOUND_OR_UNAVAILABLE = "PRODUCT_NOT_FOUND_OR_UNAVAILABLE";
    }
}
