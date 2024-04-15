require("dotenv").config();
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");
const { query, matchedData } = require("express-validator");
const normalizedTechData = require(`../${process.env.NORMALIZED_TECH_KEYWORDS}`);
const techCategories = require(`../${process.env.TECH_CATEGORIES}`);

const categories = [
  "programming_languages",
  "databases",
  "frameworks_and_libraries",
  "cloud_platforms",
  "tools",
];
const orderBy = ["asc", "desc"];

exports.getTechFrequency = [
  query("category").isString().isIn(categories).trim().escape(),
  query("order").isAlpha().isIn(orderBy).trim().escape(),
  asyncHandler(async (req, res) => {
    try {
      const { category } = req.query;
      const validatedInputs = matchedData(req);

      const technologiesQuery = db.prepare("SELECT tech_type FROM tech_skill");
      const technologies = technologiesQuery.all();

      let techPopularity = [];
      technologies.forEach((element) => {
        const tech = element["tech_type"];
        const isAdded = techPopularity.findIndex(
          (element) => element["tech_type"] === tech
        );

        if (isAdded !== -1) {
          techPopularity[isAdded]["count"] =
            techPopularity[isAdded]["count"] + 1;
        } else {
          techPopularity.push({ ...element, count: 1 });
        }
      });

      let normTechPop = [];
      techPopularity.forEach((tech) => {
        const techType = tech["tech_type"];
        const isAdded = normTechPop.findIndex(
          (element) => element["tech_type"] === techType
        );

        if (techType in normalizedTechData) {
          if (isAdded !== -1) {
            normTechPop[isAdded]["count"] =
              normTechPop[isAdded]["count"] + tech["count"];
          } else {
            normTechPop.push({ ...tech });
          }
        } else {
          for (const normalizedTech in normalizedTechData) {
            const rawData = normalizedTechData[normalizedTech];

            const isAddedNorm = normTechPop.findIndex(
              (element) => element["tech_type"] === normalizedTech
            );

            if (rawData.includes(techType) && isAddedNorm !== -1) {
              normTechPop[isAddedNorm]["count"] =
                normTechPop[isAddedNorm]["count"] + tech["count"];
            } else if (rawData.includes(techType) && isAddedNorm === -1) {
              normTechPop.push({
                ...tech,
                tech_type: normalizedTech,
              });
            }
          }
        }
      });

      let filteredTechList = [...normTechPop];
      if ("category" in validatedInputs) {
        const techList = techCategories[category];
        filteredTechList = normTechPop.filter((element) => {
          const techType = element["tech_type"];
          return techList.includes(techType);
        });
      }

      filteredTechList.sort((a, b) => {
        return a["count"] - b["count"];
      });

      res.status(200).json(filteredTechList);
    } catch (error) {
      console.error(error);
      res.status(400).json({
        message: "error",
      });
    }
  }),
];
