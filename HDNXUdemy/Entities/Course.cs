﻿namespace HDNXUdemyData.Entities
{
    public class CourseEntities : BaseEntities
    {
        public string? Title { get; set; }

        public string? ShortDescription { get; set; }

        public int IdAuthor { get; set; }

        public int TotalStudentRegister { get; set; }

        public int TotalChapter { get; set; }

        public string? Duration { get; set; }

        public int PriceOfCourse { get; set; }

        public bool IsDiscount { get; set; }

        public int PercentDiscount { get; set; }

        public bool IsFree { get; set; }

        public int TypeOfCourse { get; set; } // Khoá bình thường or tranning cùng giáo viên
        public string? Introduce { get; set; }

        public int Vote5Star { get; set; }

        public int Vote4Star { get; set; }

        public int Vote3Star { get; set; }

        public int Vote2Star { get; set; }
        public int Vote1Star { get; set; }

        public int IdCategory { get; set; }

        public string? LevelCourse { get; set; }

        public int Languages { get; set; }

        public string? Description { get; set; }

        public string? PublicId { get; set; }

        public string? PictureUrl { get; set; }

        public string? KeyVideoUpload { get; set; }

        public string? FileUrl { get; set; }
        public int? ProcessCourse { get; set; }
    }
}