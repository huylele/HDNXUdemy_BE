﻿using HDNXUdemyModel.ResponModel;

namespace HDNXUdemyServices.IServices
{
    public interface IHomeServices
    {
        Task<HomeModel> GetDataForHome(int? idUser);
    }
}