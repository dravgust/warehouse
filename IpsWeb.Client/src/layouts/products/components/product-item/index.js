import PropTypes from "prop-types";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiAvatar from "components/SuiAvatar";
import productIcon from "assets/images/qr-code.png";

function ProductItem({ isSelected, item, onClick = () => {} }) {
  return (
    <SuiBox
      component="li"
      display="flex"
      justifyContent="space-between"
      alignItems="flex-start"
      //bgColor={isSelected ? "dark" : "grey-100"}

      borderRadius="lg"
      p={2}
      mt={1}
      style={{ cursor: "pointer", border: "1px solid rgba(0, 0, 0, 0.125)" }}
      onClick={onClick}
      sx={{
        backgroundColor: isSelected ? "rgba(203, 12, 159, 0.08)" : "inherit",
        "&:hover": {
          //backgroundColor: "rgba(203, 12, 159, 0.08)",
          backgroundColor: isSelected ? "rgba(203, 12, 159, 0.08)" : "rgba(0, 0, 0, 0.04)",
        },
      }}
    >
      <SuiBox width="100%" display="flex" flexDirection="column">
        <SuiBox
          display="flex"
          justifyContent="space-between"
          alignItems={{ xs: "flex-start", sm: "center" }}
          flexDirection={{ xs: "column", sm: "row" }}
          mb={1}
        >
          <SuiBox display="flex" alignItems="center">
            <SuiBox mx={2}>
              <SuiAvatar src={productIcon} alt={item.name} size="sm" variant="square" />
            </SuiBox>
            <SuiTypography variant="button" fontWeight="medium">
              {item.name}
            </SuiTypography>
          </SuiBox>
        </SuiBox>

        <SuiBox mb={1} px={2} lineHeight={0} style={{ height: "40px" }}>
          <SuiTypography variant="caption" fontWeight="medium" textTransform="capitalize">
            {item.description}
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    </SuiBox>
  );
}

// Setting default values for the props of Bill
ProductItem.defaultProps = {
  noGutter: false,
};

// Typechecking props for the Bill
ProductItem.propTypes = {
  item: PropTypes.object.isRequired,
  noGutter: PropTypes.bool,
};

export default ProductItem;
