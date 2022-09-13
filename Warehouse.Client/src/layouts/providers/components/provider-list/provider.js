import SuiBox from "components/SuiBox";
import PersonIcon from "@mui/icons-material/Person";
import SuiTypography from "components/SuiTypography";

function Provider({ name }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <PersonIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium">
          {name || "n/a"}
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}
export default Provider;
