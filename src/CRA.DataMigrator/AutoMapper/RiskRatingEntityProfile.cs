using AutoMapper;
using CRA.DataMigration.DAL.Entities.BigQuery;
using CRA.DataMigrator.Models.Data;

namespace CRA.DataMigrator.AutoMapper
{
    public class RiskRatingEntityProfile : Profile
    {
        public RiskRatingEntityProfile()
        {
            CreateMap<RiskScoreChangedData, RiskRatingEntity>()
                .ForMember(dest=>dest.RatingTime, opts=>opts.MapFrom(s=>s.RatingTime.ToString("yyyy-MM-ddTHH:mm:ss.F")));

            CreateMap<RiskRatingFactor, RiskFactorEntity>();
        }
    }
}