function sturgesFormula(n) {
  return Math.floor(1 + 3.322 * Math.log10(n));
}

function getRange(min, max) {
  return max - min;
}

function getClassSize(range, interval) {
  return Math.round(range / interval);
}

function getFrequencyDistribution(data) {
  const sortedData = data.sort((a, b) => {
    return a - b;
  });
  const n = sortedData.length;
  let min = sortedData[0];
  const max = sortedData[n - 1];
  const classIntervals = sturgesFormula(n);
  const range = getRange(min, max);
  const classSize = getClassSize(range, classIntervals);

  let frequencyDistributionList = [];
  for (let index = 0; index < classIntervals; index++) {
    let frequencyDistribution = {};
    const intervalUpperBoundary = min + (classSize - 1);
    const range = `${min}-${intervalUpperBoundary}`;
    frequencyDistribution["range"] = range;
    frequencyDistribution["count"] = 0;
    frequencyDistributionList.push({ ...frequencyDistribution });

    min += classSize;
    if (index === classIntervals - 1 && intervalUpperBoundary < max) {
      const lastRange = `${min}-${min + (classSize - 1)}`;
      frequencyDistribution["range"] = lastRange;
      frequencyDistribution["count"] = 0;
      frequencyDistributionList.push({ ...frequencyDistribution });
    }
  }

  return frequencyDistributionList;
}

// takes array of values
function getMedian(arr) {
  const valueArray = arr.filter((value) => {
    if (typeof Number(value) === "number" && !isNaN(Number(value))) {
      return value;
    }
  });
  if (valueArray.length === 0) {
    return 0;
  }
  if (valueArray.length === 2) {
    return (valueArray[0] + valueArray[1]) / 2;
  }
  const mid = Math.floor(valueArray.length / 2);
  const median = valueArray.sort((a, b) => {
    return a - b;
  })[mid];
  return median;
}

module.exports = {
  getFrequencyDistribution,
  getMedian,
};
