import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

function Function({ job, org }) {
  return (
    <SuiBox display="flex" flexDirection="column">
      <SuiTypography variant="caption" fontWeight="medium" color="text">
        {job}
      </SuiTypography>
      <SuiTypography variant="caption" color="secondary">
        {org}
      </SuiTypography>
    </SuiBox>
  );
}
export default Function;
