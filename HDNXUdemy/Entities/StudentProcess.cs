﻿namespace HDNXUdemyData.Entities
{
    public class StudentProcessEntities : BaseEntities
    {
        public int IdCourse { get; set; }

        public int NumberContentOfCourse { get; set; }

        public int LastContentOfContent { get; set; } // Bài học sau cùng để resumn
    }
}