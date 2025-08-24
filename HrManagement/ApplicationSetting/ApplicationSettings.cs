namespace HrManagement.ApplicationSetting
{
    public static class ApplicationSettings
    {
        public static class TariningStatus
        {
            public static string Published = "1";
            public static string Draft = "2";
            public static string Locked = "3";
            public static string Drip = "4";
        }
        public static class TrainingType
        {
            public static string BlankCategory = "1";
            public static string CredentialCategory = "2";
        }

        public static class TrainingStructurType
        {
            public static string Lesson = "1";
            public static string Subcategory = "2";
            public static string Assessment = "3";
            public static string Credential = "5";
        }

        public static class ModuleName
        {
            public static string Assessment = "1";
            public static string PostDetail = "2";
            public static string TrainingDetail = "3";
            public static string Outline = "4";
        }

        public static class FileMediaStatus
        {
            public static string Active = "1";
        }
    }
}
