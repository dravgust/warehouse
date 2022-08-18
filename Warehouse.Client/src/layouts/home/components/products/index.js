import React, { useEffect } from "react";
import { Card, Icon, IconButton, Tooltip } from "@mui/material";
import SuiBox from "components/SuiBox";
import { useState } from "react";
import { useQuery } from "react-query";
import SuiTypography from "components/SuiTypography";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import ListItemIcon from "@mui/material/ListItemIcon";
import { FixedSizeList } from "react-window";
import ListItemButton from "@mui/material/ListItemButton";
import SensorsOutlinedIcon from "@mui/icons-material/SensorsOutlined";
import SuiInput from "components/SuiInput";
import { fetchAssetsInfo } from "utils/query-keys";
import { getAssetsInfo } from "services/warehouse-service";
import { useSoftUIController } from "../../../../context";
import { Accordion, AccordionSummary, AccordionDetails } from "../sites/components/accordion";
import Product from "./components/product";

export default function ProductsTreeView({
  searchTerm = "",
  selectedProduct = { beacons: [] },
  onProductSelect = () => {},
  selectedBeacon = "",
  onBeaconSelect = () => {},
}) {
  const [pattern, setPattern] = useState("");
  const [reload, updateReloadState] = useState(null);
  const [controller, dispatch] = useSoftUIController();
  const [expanded, setExpanded] = React.useState("");
  const { direction } = controller;
  const onSearchProduct = (productItem) => setPattern(productItem);

  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchAssetsInfo, reload], getAssetsInfo);

  const handleChange = (panel, row) => (event, newExpanded) => {
    setExpanded(newExpanded ? panel : false);
    setPattern("");
    onBeaconSelect("");
    onProductSelect(row);
  };
  let assets =
    (selectedProduct &&
      selectedProduct.beacons.filter((b) => {
        return Boolean(
          !pattern ||
            b.beacon.macAddress.toLocaleUpperCase().indexOf(pattern.toLocaleUpperCase()) > -1
        );
      })) ||
    [];

  const forceUpdate = () => {
    setExpanded("");
    onBeaconSelect("");
    onProductSelect(null);
    updateReloadState(Date.now());
  };

  useEffect(() => {
    console.log(selectedProduct);
    selectedProduct && setExpanded(`panel_${selectedProduct.product.id}`);
  }, [isSuccess]);

  const Row = ({ index, style }) => (
    <ListItem
      key={`b_${index}`}
      style={style}
      component="div"
      disablePadding
      onClick={() => onBeaconSelect(assets[index].beacon)}
      sx={{
        borderBottom: ({ borders: { borderWidth, borderColor } }) =>
          `${borderWidth[1]} solid ${borderColor}`,
      }}
      selected={assets[index].beacon.macAddress === selectedBeacon.macAddress}
      secondaryAction={
        <SuiTypography
          variant="h6"
          fontWeight="medium"
          color={assets[index].site.name ? "info" : "secondary"}
          mx={2}
        >
          {assets[index].site.name || "n/a"}
        </SuiTypography>
      }
    >
      <ListItemButton dir={direction}>
        <ListItemIcon>
          <SensorsOutlinedIcon />
        </ListItemIcon>
        <ListItemText
          primaryTypographyProps={{ color: assets[index].beacon.name ? "dark" : "secondary" }}
          primary={assets[index].beacon.name || "n/a"}
          secondary={
            <React.Fragment>
              <SuiTypography
                sx={{ display: "inline" }}
                component="span"
                variant="caption"
                color="secondary"
              >
                {assets[index].beacon.macAddress}
              </SuiTypography>
            </React.Fragment>
          }
        />
      </ListItemButton>
    </ListItem>
  );

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Products
          </SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
          <IconButton size="xl" color="inherit" onClick={forceUpdate}>
            <Tooltip title="Reload">
              <Icon>sync</Icon>
            </Tooltip>
          </IconButton>
        </SuiBox>
      </SuiBox>
      <SuiBox pb={3}>
        {isSuccess &&
          response.map((item, index) => (
            <Accordion
              expanded={expanded === `panel_${item.product.id}`}
              onChange={handleChange(`panel_${item.product.id}`, item)}
              key={`product_${index}`}
              TransitionProps={{ unmountOnExit: true }}
            >
              <AccordionSummary
                aria-controls={`panel_${item.product.id}_content`}
                id={`panel_${item.product.id}_header`}
                sx={{ "& .MuiAccordionSummary-content": { margin: "7px 0" } }}
              >
                <SuiBox
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  style={{ width: "100%" }}
                >
                  <Product product={item.product} count={item.beacons.length}></Product>
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
                    height: "350px",
                  }}
                >
                  <FixedSizeList
                    className="List"
                    height={350}
                    itemCount={assets.length}
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
