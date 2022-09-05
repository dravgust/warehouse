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
import TabOutlinedIcon from "@mui/icons-material/TabOutlined";
import SuiInput from "components/SuiInput";
import { fetchAssetsInfo } from "utils/query-keys";
import { getAssetsInfo } from "api/warehouse";
import { useSoftUIController } from "../../../../context";
import { Accordion, AccordionSummary, AccordionDetails } from "../sites/components/accordion";
import Product from "./components/product";
import OpenInNewIcon from "@mui/icons-material/OpenInNew";
import { useNavigate } from "react-router-dom";

export default function ProductsTreeView({
  searchTerm = "",
  selectedProduct = { beacons: [] },
  onProductSelect = () => {},
  selectedSite = "",
  onSiteSelect = () => {},
}) {
  const [pattern, setPattern] = useState("");
  const [reload, updateReloadState] = useState(null);
  const [controller, dispatch] = useSoftUIController();
  const [expanded, setExpanded] = React.useState("");
  const { direction } = controller;
  const navigate = useNavigate();
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
    onSiteSelect(null);
    onProductSelect(row);
  };
  let assets =
    (selectedProduct &&
      selectedProduct.sites.filter((b) => {
        return Boolean(
          !pattern || b.name.toLocaleUpperCase().indexOf(pattern.toLocaleUpperCase()) > -1
        );
      })) ||
    [];

  const forceUpdate = () => {
    setExpanded("");
    onSiteSelect(null);
    onProductSelect(null);
    updateReloadState(Date.now());
  };

  useEffect(() => {
    selectedProduct && setExpanded(`panel_${selectedProduct.id}`);
  }, [isSuccess]);

  const handleClick = (e, index) => {
    onSiteSelect(assets[index]);
    switch (e.detail) {
      case 2:
        navigate("/warehouse");
        break;
    }
  };

  const Row = ({ index, style }) => (
    <ListItem
      key={`b_${index}`}
      style={style}
      component="div"
      disablePadding
      onClick={(e) => handleClick(e, index)}
      onDoubleClick={(e) => handleClick(e, index)}
      sx={{
        borderBottom: ({ borders: { borderWidth, borderColor } }) =>
          `${borderWidth[1]} solid ${borderColor}`,
      }}
      selected={selectedSite && assets[index].id === selectedSite.id}
      secondaryAction={
        selectedSite &&
        assets[index].id === selectedSite.id && (
          <IconButton edge="start" onClick={() => navigate("/warehouse")}>
            <OpenInNewIcon />
          </IconButton>
        )
      }
    >
      <ListItemButton dir={direction}>
        <ListItemIcon>
          <TabOutlinedIcon fontSize="large" />
        </ListItemIcon>
        <ListItemText
          primaryTypographyProps={{ color: "#17c1e8", fontSize: "0.875rem" }}
          primary={assets[index].name}
          secondaryTypographyProps={{ color: "#8392ab", fontSize: "0.75rem" }}
          secondary={`${assets[index].beacons.length} items`}
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
      <SuiBox pb={3} px={2}>
        {isSuccess &&
          response.map((item, index) => (
            <Accordion
              expanded={expanded === `panel_${item.id}`}
              onChange={handleChange(`panel_${item.id}`, item)}
              key={`product_${index}`}
              TransitionProps={{ unmountOnExit: true }}
            >
              <AccordionSummary
                aria-controls={`panel_${item.id}_content`}
                id={`panel_${item.id}_header`}
                sx={{ "& .MuiAccordionSummary-content": { margin: "7px 0" } }}
              >
                <SuiBox
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  style={{ width: "100%" }}
                >
                  <Product {...item}></Product>
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
