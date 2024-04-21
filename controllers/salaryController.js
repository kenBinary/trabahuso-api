const asyncHandler = require("express-async-handler");
const stat = require("../utils/statUtils");
const client = require("../helpers/turso");

exports.getSalary = asyncHandler(async (req, res) => {
  try {
    let salaryDetails = {
      undisclosed: null,
      disclosed: null,
    };

    const salaryListQuery = await client.execute(
      "SELECT salary FROM job_data where salary is not null"
    );
    const salaryList = salaryListQuery.rows
      .map((salaryObj) => {
        return salaryObj["salary"];
      })
      .sort((a, b) => {
        return a - b;
      });

    const nullSalaryListQuery = await client.execute(
      "SELECT salary FROM job_data where salary is null"
    );

    let frequencyDistribution = stat.getFrequencyDistribution(salaryList);

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

    salaryDetails.undisclosed = nullSalaryListQuery.rows.length;
    salaryDetails.disclosed = frequencyDistribution;

    res.status(200).json(salaryDetails);
  } catch (error) {
    console.error(error);
    res.status(400).send("error");
  }
});
