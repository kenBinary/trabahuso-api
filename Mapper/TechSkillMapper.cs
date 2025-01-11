using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using trabahuso_api.DTOs.TechSkill;
using trabahuso_api.Models;

namespace trabahuso_api.Mapper
{
    public static class TechSkillMapper
    {
        public static List<TechSkill> ToTechSkills(this List<List<Row>> rows)
        {
            List<TechSkill> techSkills = [];
            foreach (List<Row> row in rows)
            {

                Row x = row[0];
                Row y = row[1];
                Row z = row[2];

                if (x.Value != null && y.Value != null && z.Value != null)
                {
                    techSkills.Add(
                        new TechSkill()
                        {
                            TechStackId = x.Value,
                            JobDataId = y.Value,
                            TechType = z.Value,
                        }
                    );
                }

            }

            return techSkills;
        }

        public static TechSkill? ToTechSkill(this List<Row> row)
        {

            Row x = row[0];
            Row y = row[1];
            Row z = row[2];

            if (x.Value != null && y.Value != null && z.Value != null)
            {
                return new TechSkill()
                {
                    TechStackId = x.Value,
                    JobDataId = y.Value,
                    TechType = z.Value,
                };
            }

            return null;
        }

        public static TechSkillDto ToTechSkillDto(this TechSkill techSkill)
        {
            return new TechSkillDto(techSkill.TechStackId, techSkill.TechType);
        }
    }
}