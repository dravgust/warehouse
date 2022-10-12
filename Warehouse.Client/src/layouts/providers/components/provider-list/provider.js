import SuiBox from "components/SuiBox";
import FactoryIcon from "@mui/icons-material/Factory";
import SuiTypography from "components/SuiTypography";

function Provider({ name, alias }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <FactoryIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium">
          {name || "n/a"}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {alias}
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}
export default Provider;
