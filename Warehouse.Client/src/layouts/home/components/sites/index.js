import * as React from "react";
import { Card, Icon, IconButton, Tooltip } from "@mui/material";
import SuiBox from "components/SuiBox";
import { useEffect, useState } from "react";
import { useQuery } from "react-query";
import SuiTypography from "components/SuiTypography";
import ListItem from "@mui/material/ListItem";
import ListItemText from "@mui/material/ListItemText";
import ListItemIcon from "@mui/material/ListItemIcon";
import { FixedSizeList } from "react-window";
import ListItemButton from "@mui/material/ListItemButton";
import QrCode2SharpIcon from "@mui/icons-material/QrCode2Sharp";
import SuiInput from "components/SuiInput";
import { fetchSitesInfo } from "utils/query-keys";
import { getSitesInfo } from "services/warehouse-service";
import { useSoftUIController } from "../../../../context";
import { Accordion, AccordionSummary, AccordionDetails } from "./components/accordion";
import Site from "./components/site";
import OpenInNewIcon from "@mui/icons-material/OpenInNew";
import { useNavigate } from "react-router-dom";

export default function SiteInfo({
  searchTerm = "",
  selectedSite = { products: [] },
  onSiteSelect = () => {},
  selectedProduct = "",
  onProductSelect = () => {},
}) {
  const [pattern, setPattern] = useState("");
  const [controller] = useSoftUIController();
  const [reload, updateReloadState] = useState(null);
  const [expanded, setExpanded] = React.useState("");
  const navigate = useNavigate();
  const { direction } = controller;

  const onSearchProduct = (productItem) => setPattern(productItem);
  const forceUpdate = () => {
    setExpanded("");
    onProductSelect(null);
    onSiteSelect(null);
    updateReloadState(Date.now());
  };
  const handleChange = (panel, row) => (event, newExpanded) => {
    setExpanded(newExpanded ? panel : false);
    setPattern("");
    onProductSelect(null);
    onSiteSelect(row);
  };

  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchSitesInfo, reload], getSitesInfo);

  useEffect(() => {
    selectedSite && setExpanded(`panel_${selectedSite.id}`);
  }, [isSuccess]);

  let assets =
    (selectedSite &&
      selectedSite.products.filter((b) => {
        return Boolean(
          !pattern || b.name.toLocaleUpperCase().indexOf(pattern.toLocaleUpperCase()) > -1
        );
      })) ||
    [];

  const Row = ({ index, style }) => (
    <ListItem
      key={`b_${index}`}
      style={style}
      component="div"
      disablePadding
      onClick={() => onProductSelect(assets[index])}
      sx={{
        borderBottom: ({ borders: { borderWidth, borderColor } }) =>
          `${borderWidth[1]} solid ${borderColor}`,
      }}
      selected={selectedProduct && assets[index].id === selectedProduct.id}
      secondaryAction={
        <IconButton edge="start" onClick={() => navigate("/warehouse")}>
          <OpenInNewIcon />
        </IconButton>
      }
    >
      <ListItemButton dir={direction}>
        <ListItemIcon>
          <QrCode2SharpIcon fontSize="large" />
        </ListItemIcon>
        <ListItemText
          primaryTypographyProps={{ color: "#cb0c9f", fontSize: "0.875rem" }}
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
            Sites
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
              expanded={expanded === `panel_${item.id}`}
              onChange={handleChange(`panel_${item.id}`, item)}
              key={`site_${index}`}
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
                  <Site {...item}></Site>
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
