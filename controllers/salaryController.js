require("dotenv").config();
const asyncHandler = require("express-async-handler");
const db = require("../helpers/dbConnection");
const stat = require("../utils/statUtils");

exports.getSalary = asyncHandler(async (req, res) => {
  try {
    let salaryDetails = {
      undisclosed: null,
      disclosed: null,
    };

    const salaryListQuery = db.prepare(
      "SELECT salary FROM job_data where salary is not null"
    );
    salaryListQuery.raw();
    const salaryList = salaryListQuery
      .all()
      .flat()
      .sort((a, b) => a - b);

    const nullSalaryListQuery = db.prepare(
      "SELECT salary FROM job_data where salary is null"
    );
    nullSalaryListQuery.raw();
    const nullSalaryList = nullSalaryListQuery.all().flat();

    let frequencyDistribution = stat.getFrequencyDistribution(salaryList);
    console.log(frequencyDistribution);

    frequencyDistribution.forEach((distribution, index) => {
      const [min, max] = distribution.range.split("-");
      let rangeCount = 0;
      salaryList.forEach((salary) => {
        if (salary >= Number(min) && salary <= max) {
          rangeCount += 1;
        }
      });
      frequencyDistribution[index].count = rangeCount;
    });

    salaryDetails.undisclosed = nullSalaryList.length;
    salaryDetails.disclosed = frequencyDistribution;

    res.status(200).json(salaryDetails);
  } catch (error) {
    console.error(error);
    res.status(400).send("error");
  }
});
