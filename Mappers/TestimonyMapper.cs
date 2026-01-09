using g_flame_youth.DTOs.Account;
using g_flame_youth.DTOs.Testimony;
using g_flame_youth.Models;

namespace g_flame_youth.Mappers
{
    public static class TestimonyMapper
    {
        public static TestimonyResponseDto ToTestimonyResponseDto(this Testimony testimonyModel)
        {
            return new TestimonyResponseDto
            {
                Id = testimonyModel.Id,
                AppUserId = testimonyModel.AppUserId,
                Content = testimonyModel.Content,
                Attachment = testimonyModel.Attachment,
                Status = testimonyModel.Status,
                CreatedAt = testimonyModel.CreatedAt,

                User = new UserInfoDto
                {
                    Id = testimonyModel.User.Id,
                    FullName = testimonyModel.User.FullName,
                    Email = testimonyModel.User.Email
                }
            };
        }

        public static Testimony ToTestimonyFromCreateDto(this CreateTestimonyDto createDto)
        {
            return new Testimony
            {
                Content = createDto.Content,
                Attachment = createDto.Attachment,
            };
        }
    }
}