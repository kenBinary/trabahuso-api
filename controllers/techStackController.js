require("dotenv").config();
const asyncHandler = require("express-async-handler");
const { query, matchedData } = require("express-validator");
const client = require("../helpers/turso");
const normalizedTechData = require("../data/normalized_tech_keywords.json");
const techCategories = require("../data/tech_categories.json");

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
  query("limit").isNumeric().trim().escape(),
  asyncHandler(async (req, res) => {
    try {
      // TODO: refactor entire code
      const validatedInputs = matchedData(req);

      const technologiesQuery = await client.execute(
        "SELECT tech_type FROM tech_skill"
      );
      const technologies = technologiesQuery.rows;

      const techData = {
        data: [],
        limitCount: 0,
        totalCount: 0,
      };

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
        const techList = techCategories[validatedInputs.category];
        filteredTechList = normTechPop.filter((element) => {
          const techType = element["tech_type"];
          return techList.includes(techType);
        });
        if ("order" in validatedInputs) {
          switch (validatedInputs.order) {
            case "asc":
              filteredTechList.sort((a, b) => {
                return a["count"] - b["count"];
              });
              break;
            case "desc":
              filteredTechList.sort((a, b) => {
                return b["count"] - a["count"];
              });
              break;
            default:
              break;
          }
        }
      }
      techData.data = filteredTechList;

      techData.totalCount = filteredTechList.length;
      if ("limit" in validatedInputs) {
        techData.data = techData.data.slice(0, validatedInputs.limit);
      }
      techData.limitCount = techData.data.length;

      res.status(200).json(techData);
    } catch (error) {
      console.error(error);
      res.status(400).json({
        message: "error",
      });
    }
  }),
];
