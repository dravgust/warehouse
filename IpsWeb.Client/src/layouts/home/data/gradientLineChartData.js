const gradientLineChartData = {
  labels: ["Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
  datasets: [
    {
      label: "Site Average",
      color: "info",
      data: [50, 40, 30, 22, 50, 25, 40, 23, 50],
    },
    {
      label: "Beacon",
      color: "error",
      data: [30, 90, 40, 14, 29, 29, 34, 23, 40],
    },
  ],
};

export default gradientLineChartData;
