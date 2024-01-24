﻿using NodaTime;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HDNXUdemyData.Entities
{
    public class BaseEntities
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("Id")]
        public long Id { get; set; }

        [Column("CreateDate")]
        public LocalDateTime CreateDate { get; set; }

        [Column("UpdateDate")]
        public LocalDateTime UpdateDate { get; set; }

        [Column("CreateBy")]
        public int CreateBy { get; set; }

        [Column("UpdateBy")]
        public int UpdateBy { get; set; }

        [Column("Status")]
        public int Status { get; set; }
    }
}