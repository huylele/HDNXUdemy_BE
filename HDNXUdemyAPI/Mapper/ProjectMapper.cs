﻿using AutoMapper;
using DNXUdemyData.Entities;
using HDNXUdemyData.Entities;
using HDNXUdemyData.Model;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyServices.CommonFunction;

namespace HDNXUdemyAPI.Mapper
{
    /// <summary>
    /// ProjectMapper
    /// </summary>
    public class ProjectMapper : Profile
    {
        /// <summary>
        /// ProjectMapper
        /// </summary>
        public ProjectMapper()
        {
            CreateMap<NotificationEntities, NotificationModel>().ReverseMap();

            CreateMap<BookmarkCourseEntities, BookmarkCourseModel>().ReverseMap();
            CreateMap<CategoryEntities, CategoryModel>().ReverseMap();
            CreateMap<ChapterCommentEntities, ChapterCommentModel>().ReverseMap();
            CreateMap<ContentCourseEntities, ContentCourseModel>().ReverseMap();
            CreateMap<ContentCourseDetailEntities, ContentCourseDetailModel>().ReverseMap();
            CreateMap<BannerEntities, BannerModel>().ReverseMap();
            CreateMap<CourseEntities, CourseModel>().ReverseMap();
            CreateMap<CourseCommentEntities, CourseCommentModel>().ReverseMap();
            CreateMap<InformationManualBankingEntities, InformationManualBankingModel>().ReverseMap();
            CreateMap<PurcharseCourseEntities, PurcharseCourseModel>().ReverseMap();
            CreateMap<StudentProcessEntities, StudentProcessModel>().ReverseMap();
            CreateMap<StudentPromotionEntities, StudentPromotionModel>().ReverseMap();
            CreateMap<SubCategoryEntities, SubCategoryModel>().ReverseMap();
            CreateMap<FileManagerEntities, FileManagerModel>().ReverseMap();
            CreateMap<SystemConfigEntities, SystemConfigModel>().ReverseMap();
            CreateMap<PartnerEntities, PartnerModel>().ReverseMap();
            CreateMap<UserEntities, UserModel>().ReverseMap();

            CreateMap<ListContentWithDetailCourse, ContentCourseModel>().ReverseMap();
            CreateMap<ListContentWithDetailCourse, ContentCourseEntities>().ReverseMap();
            CreateMap<GetCourseWithDetailsContent, CourseEntities>().ReverseMap();
            CreateMap<ContentAndContentDetail, ContentCourseEntities>().ReverseMap();
            CreateMap<ResponeLogin, UserEntities>();
            CreateMap<UserEntities, ResponeLogin>()
                .ForMember(dest => dest.RoleName, opt => opt.MapFrom(x => ((ERoles)x.RoleId).GetEnumDescription()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(x => x.Id));
        }
    }
}