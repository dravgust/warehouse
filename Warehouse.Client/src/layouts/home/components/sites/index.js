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
import SensorsOutlinedIcon from "@mui/icons-material/SensorsOutlined";
import SuiInput from "components/SuiInput";
import { fetchSitesInfo } from "utils/query-keys";
import { getSitesInfo } from "services/warehouse-service";
import { useSoftUIController } from "../../../../context";
import { Accordion, AccordionSummary, AccordionDetails } from "./components/accordion";
import Site from "./components/site";

export default function SiteInfo({
  searchTerm = "",
  selectedSite = { in: [] },
  onSiteSelect = () => {},
  selectedBeacon = "",
  onBeaconSelect = () => {},
}) {
  const [pattern, setPattern] = useState("");
  const [controller] = useSoftUIController();
  const [reload, updateReloadState] = useState(null);
  const [expanded, setExpanded] = React.useState("");

  const { direction } = controller;

  const onSearchProduct = (productItem) => setPattern(productItem);
  const forceUpdate = () => {
    setExpanded("");
    onBeaconSelect("");
    onSiteSelect(null);
    updateReloadState(Date.now());
  };
  const handleChange = (panel, row) => (event, newExpanded) => {
    setExpanded(newExpanded ? panel : false);
    setPattern("");
    onBeaconSelect("");
    onSiteSelect(row);
  };

  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchSitesInfo, reload], getSitesInfo);

  useEffect(() => {
    selectedSite && setExpanded(`panel_${selectedSite.site.id}`);
  }, [isSuccess]);

  let assets =
    (selectedSite &&
      selectedSite.in.filter((b) => {
        return Boolean(
          !pattern ||
            b.beacon.macAddress.toLocaleUpperCase().indexOf(pattern.toLocaleUpperCase()) > -1
        );
      })) ||
    [];

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
          color={assets[index].product.name ? "primary" : "secondary"}
          mx={2}
        >
          {assets[index].product.name || "n/a"}
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
              expanded={expanded === `panel_${item.site.id}`}
              onChange={handleChange(`panel_${item.site.id}`, item)}
              key={`site_${index}`}
              TransitionProps={{ unmountOnExit: true }}
            >
              <AccordionSummary
                aria-controls={`panel_${item.site.id}_content`}
                id={`panel_${item.site.id}_header`}
                sx={{ "& .MuiAccordionSummary-content": { margin: "7px 0" } }}
              >
                <SuiBox
                  display="flex"
                  justifyContent="space-between"
                  alignItems="center"
                  style={{ width: "100%" }}
                >
                  <Site site={item.site} count={item.in.length}></Site>
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
