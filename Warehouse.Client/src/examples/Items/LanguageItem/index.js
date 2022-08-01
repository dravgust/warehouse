/**
=========================================================
* Soft UI Dashboard React - v3.1.0
=========================================================

* Product Page: https://www.creative-tim.com/product/soft-ui-dashboard-react
* Copyright 2022 Creative Tim (https://www.creative-tim.com)

Coded by www.creative-tim.com

 =========================================================

* The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
*/

import { forwardRef } from "react";

// prop-types is a library for typechecking of props.
import PropTypes from "prop-types";

// @mui material components
import MenuItem from "@mui/material/MenuItem";

// Soft UI Dashboard React components
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";

// custom styles for the LanguageItem
import { menuItem, menuImage } from "examples/Items/LanguageItem/styles";

const LanguageItem = forwardRef(({ color, image, title, date, ...rest }, ref) => (
  <MenuItem {...rest} ref={ref} sx={(theme) => menuItem(theme)}>
    <SuiBox
      width="2rem"
      height="2rem"
      mt={0.25}
      mr={2}
      mb={0.25}
      borderRadius="lg"
      sx={(theme) => menuImage(theme, { color })}
    >
      {image}
    </SuiBox>
    <SuiBox>
      <SuiTypography variant="button" textTransform="capitalize" fontWeight="regular">
        {title[0]}
      </SuiTypography>
    </SuiBox>
  </MenuItem>
));

// Setting default values for the props of LanguageItem
LanguageItem.defaultProps = {
  color: "dark",
};

// Typechecking props for the LanguageItem
LanguageItem.propTypes = {
  color: PropTypes.oneOf([
    "primary",
    "secondary",
    "info",
    "success",
    "warning",
    "error",
    "light",
    "dark",
  ]),
  image: PropTypes.node.isRequired,
  title: PropTypes.arrayOf(PropTypes.string).isRequired,
};

export default LanguageItem;
