﻿using AutoMapper;
using HDNXUdemyData.Entities;
using HDNXUdemyData.IRepository;
using HDNXUdemyModel.Constant;
using HDNXUdemyModel.Model;
using HDNXUdemyModel.ResponModel;
using HDNXUdemyModel.SystemExceptions;
using HDNXUdemyServices.CommonFunction;
using HDNXUdemyServices.IServices;
using Microsoft.AspNetCore.SignalR;

namespace HDNXUdemyServices.Services
{
    public class PurcharseCourseServices : IPurcharseCourseServices
    {
        private readonly IPurcharseCourseRepository _purcharseCourseRepository;
        private readonly IRPPurcharseCourseDetailsRepository _purcharseCourseDetailsRepository;
        private readonly IMapper _mapper;
        private readonly IHubContext<HubConfigProject> _hubConfigProject;
        private readonly ICourseRepository _courseRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IInformationManualBankingRepository _informationManualBankingRepository;

        public PurcharseCourseServices(IPurcharseCourseRepository purcharseCourseRepository, IMapper mapper, IHubContext<HubConfigProject> hubConfigProject,
            ICourseRepository courseRepository, INotificationRepository notificationRepository, IInformationManualBankingRepository informationManualBankingRepository,
            IRPPurcharseCourseDetailsRepository purcharseCourseDetailsRepository)
        {
            _purcharseCourseRepository = purcharseCourseRepository ?? throw new ProjectException(nameof(_purcharseCourseRepository));
            _mapper = mapper ?? throw new ProjectException(nameof(_mapper));
            _hubConfigProject = hubConfigProject ?? throw new ProjectException(nameof(_hubConfigProject));
            _courseRepository = courseRepository ?? throw new ProjectException(nameof(_courseRepository));
            _notificationRepository = notificationRepository ?? throw new ProjectException(nameof(_notificationRepository));
            _informationManualBankingRepository = informationManualBankingRepository ?? throw new ProjectException(nameof(_informationManualBankingRepository));
            _purcharseCourseDetailsRepository = purcharseCourseDetailsRepository ?? throw new ProjectException(nameof(_purcharseCourseDetailsRepository));
        }

        public string GenPurchaseOrder(int idStudent)
        {
            return Generator.GenerateRandomString(idStudent);
        }

        public async Task<PurcharseCourseModel> CreateRequestPurchase(PurcharseCourseModel model)
        {
            var getValueOfInfoPurechase = _mapper.Map<InformationManualBankingModel>(
                (await _informationManualBankingRepository.GetAsync(x => x.Status == (int)EStatus.Active)).FirstOrDefault());

            model.PurcharseStatus = (int)ETypeOfStatusOrder.Request;
            var dataInsert = _mapper.Map<PurcharseCourseEntities>(model);
            var addReturnModel = await _purcharseCourseRepository.AddReturnModelAsync(dataInsert);

            model.ListPurchaseCourseDetails.ForEach(item =>
            {
                item.IdPurchaseOrder = (int)addReturnModel.Id;
            });
            var dataInsertDetail = _mapper.Map<List<PurcharseCourseDetailsEntities>>(model.ListPurchaseCourseDetails);
            await _purcharseCourseDetailsRepository.AddManyAsync(dataInsertDetail);

            return model;
        }

        public async Task<bool> UpdateStatusPurchase(int id, PurcharseCourseModel model)
        {
            var getData = await _purcharseCourseRepository.GetByIdAsync(id) ?? new PurcharseCourseEntities();
            getData.Status = model.Status;
            getData.PurcharseStatus = model.PurcharseStatus;
            bool returnValue = await _purcharseCourseRepository.UpdateAsync(getData);
            if (returnValue)
            {
                var getDataOfCourse = await _courseRepository.GetObjectAsync(id);
                var dataInsertNotification = new NotificationEntities()
                {
                    IdCourse = (int)(getDataOfCourse?.Id ?? 0),
                    ShortComment = $"Khoá học {getDataOfCourse?.Title} đã thanh toán thành công",
                    IdStudent = model.IdStudent,
                    IsRead = false,
                    TypeNotification = (int)TypeNotification.UpdateOnCourse,
                };

                var insertNotification = await _notificationRepository.AddReturnModelAsync(dataInsertNotification);
                var contentNotification = new InfoNotification()
                {
                    IdNotification = insertNotification.Id.ToString(),
                    IsRead = insertNotification.IsRead,
                    NotificationContent = insertNotification.ShortComment,
                };
                await _hubConfigProject.Clients.Client(model.IdStudent.ToString())
                    .SendAsync(TypeNotification.UpdateOnCourse.GetEnumDescription(), contentNotification);
            }
            return returnValue;
        }
    }
}