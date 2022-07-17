import * as React from "react";
import { Card } from "@mui/material";
import SuiBox from "components/SuiBox";
import { useState } from "react";
import { useQuery } from "react-query";
import SuiTypography from "components/SuiTypography";
import { styled } from "@mui/material/styles";
import ArrowForwardIosSharpIcon from "@mui/icons-material/ArrowForwardIosSharp";
import MuiAccordion from "@mui/material/Accordion";
import MuiAccordionSummary from "@mui/material/AccordionSummary";
import MuiAccordionDetails from "@mui/material/AccordionDetails";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import ListItemIcon from "@mui/material/ListItemIcon";
import { FixedSizeList } from "react-window";
import ListItemButton from "@mui/material/ListItemButton";
import QrCode2SharpIcon from "@mui/icons-material/QrCode2Sharp";
import SensorsOutlinedIcon from "@mui/icons-material/SensorsOutlined";
import SuiInput from "components/SuiInput";
import Menu from "@mui/material/Menu";
import MenuItem from "@mui/material/MenuItem";
import Icon from "@mui/material/Icon";
import { fetchAssetsInfo } from "utils/query-keys";
import { getAssetsInfo } from "services/warehouse-service";

const Accordion = styled((props) => (
  <MuiAccordion disableGutters elevation={0} square {...props} />
))(({ theme }) => ({
  borderTop: `1px solid ${theme.borders.borderColor}`,
  "&:not(:last-child)": {
    borderBottom: 0,
  },
  "&:first-of-type": {
    borderTop: 0,
  },
  "&:before": {
    display: "none",
  },
}));

const AccordionSummary = styled((props) => (
  <MuiAccordionSummary
    expandIcon={<ArrowForwardIosSharpIcon sx={{ fontSize: "0.9rem" }} />}
    {...props}
  />
))(({ theme }) => ({
  backgroundColor: "transparent",
  //flexDirection: 'row-reverse',
  "& .MuiAccordionSummary-expandIconWrapper.Mui-expanded": {
    transform: "rotate(90deg)",
  },
  "& .MuiAccordionSummary-content": {
    //marginLeft: theme.spacing(1),
  },
}));

const AccordionDetails = styled(MuiAccordionDetails)(({ theme }) => ({
  padding: theme.spacing(2),
  borderTop: "1px solid rgba(0, 0, 0, .125)",
  backgroundColor: "aliceblue",
  // backgroundColor: '#f8f9fa'
}));

function Product({ product, count }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <QrCode2SharpIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography
          variant="button"
          fontWeight="medium"
          color={product.name ? "primary" : "secondary"}
        >
          {product.name || "Undefined"}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {count}&nbsp;items
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}

function Site({ site }) {
  return (
    <SuiBox display="flex" alignItems="center" px={2}>
      <SuiTypography variant="h6" color="info">
        {site.name}
      </SuiTypography>
    </SuiBox>
  );
}

export default function ProductsTreeView({
  searchTerm = "",
  selectedProduct = { beacons: [] },
  onProductSelect = () => {},
  selectedBeacon = "",
  onBeaconSelect = () => {},
  onListSelect = () => {},
}) {
  const [pattern, setPattern] = useState("");
  const onSearchProduct = (productItem) => setPattern(productItem);

  let beacons =
    (selectedProduct &&
      selectedProduct.beacons.filter((b) => {
        return Boolean(
          !pattern || b.macAddress.toLocaleUpperCase().indexOf(pattern.toLocaleUpperCase()) > -1
        );
      })) ||
    [];

  const [menu, setMenu] = useState(null);

  const openMenu = ({ currentTarget }) => setMenu(currentTarget);
  const closeMenu = () => setMenu(null);

  const renderMenu = (
    <Menu
      id="simple-menu"
      anchorEl={menu}
      anchorOrigin={{
        vertical: "top",
        horizontal: "left",
      }}
      transformOrigin={{
        vertical: "top",
        horizontal: "right",
      }}
      open={Boolean(menu)}
      onClose={closeMenu}
    >
      <MenuItem
        onClick={() => {
          closeMenu();
          onListSelect("beacon");
        }}
      >
        Beacon List
      </MenuItem>
      <MenuItem
        onClick={() => {
          closeMenu();
          onListSelect("site");
        }}
      >
        Site List
      </MenuItem>
    </Menu>
  );

  const Row = ({ index, style }) => (
    <ListItem
      key={`b_${index}`}
      style={style}
      component="div"
      disablePadding
      onClick={() => onBeaconSelect(beacons[index])}
      sx={{
        borderBottom: ({ borders: { borderWidth, borderColor } }) =>
          `${borderWidth[1]} solid ${borderColor}`,
      }}
      selected={beacons[index].macAddress === selectedBeacon.macAddress}
    >
      <ListItemButton>
        <ListItemIcon>
          <SensorsOutlinedIcon />
        </ListItemIcon>
        <ListItemText
          primaryTypographyProps={{ color: "dark" }}
          primary={beacons[index].name || "n/a"}
          secondary={
            <React.Fragment>
              <SuiTypography
                sx={{ display: "inline" }}
                component="span"
                variant="caption"
                color="secondary"
              >
                {beacons[index].macAddress}
              </SuiTypography>
            </React.Fragment>
          }
        />
      </ListItemButton>
    </ListItem>
  );

  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchAssetsInfo], getAssetsInfo);

  const [expanded, setExpanded] = React.useState("");

  const handleChange = (panel, row) => (event, newExpanded) => {
    setExpanded(newExpanded ? panel : false);
    setPattern("");
    onBeaconSelect("");
    onProductSelect(row);
  };

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Products
          </SuiTypography>
        </SuiBox>
        <SuiBox color="text" px={2}>
          <Icon sx={{ cursor: "pointer", fontWeight: "bold" }} fontSize="small" onClick={openMenu}>
            more_vert
          </Icon>
        </SuiBox>
        {renderMenu}
      </SuiBox>
      <SuiBox pb={3}>
        {isSuccess &&
          response.map((item, index) => (
            <Accordion
              expanded={expanded === `panel_${index}`}
              onChange={handleChange(`panel_${index}`, item)}
              key={`product_${index}`}
              TransitionProps={{ unmountOnExit: true }}
            >
              <AccordionSummary
                aria-controls={`panel_${index}_content`}
                id={`panel_${index}_header`}
                sx={{ "& .MuiAccordionSummary-content": { margin: "7px 0" } }}
              >
                <SuiBox
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  style={{ width: "100%" }}
                >
                  <Product product={item.product} count={item.beacons.length} />
                  <Site site={item.site}></Site>
                </SuiBox>
              </AccordionSummary>
              <AccordionDetails>
                <SuiBox py={1}>
                  <SuiInput
                    className="search-products"
                    placeholder="Type here..."
                    icon={{
                      component: "search",
                      direction: "left",
                    }}
                    onChange={(event) => onSearchProduct(event.target.value)}
                  />
                </SuiBox>
                <SuiBox
                  component="ul"
                  display="flex"
                  flexDirection="column"
                  sx={{
                    height: "300px",
                  }}
                >
                  <FixedSizeList
                    className="List"
                    height={350}
                    itemCount={beacons.length}
                    itemSize={65}
                  >
                    {Row}
                  </FixedSizeList>
                </SuiBox>
              </AccordionDetails>
            </Accordion>
          ))}
        {isLoading && (
          <SuiTypography px={2} color="secondary">
            Loading..
          </SuiTypography>
        )}
        {error && (
          <SuiTypography px={2} color="error">
            Error occurred!
          </SuiTypography>
        )}
      </SuiBox>
    </Card>
  );
}
