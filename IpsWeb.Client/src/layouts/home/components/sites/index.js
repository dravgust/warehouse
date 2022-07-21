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
import SensorsOutlinedIcon from "@mui/icons-material/SensorsOutlined";
import TabOutlinedIcon from "@mui/icons-material/TabOutlined";
import SuiInput from "components/SuiInput";
import { fetchSitesInfo } from "utils/query-keys";
import { getSitesInfo } from "services/warehouse-service";

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

function Site({ site, count }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <TabOutlinedIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium" color={"info"}>
          {site.name || "Undefined"}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {count}&nbsp;items
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}

export default function SiteInfo({
  searchTerm = "",
  selectedSite = { beacons: [] },
  onSiteSelect = () => {},
  selectedBeacon = "",
  onBeaconSelect = () => {},
}) {
  const [pattern, setPattern] = useState("");
  const onSearchProduct = (productItem) => setPattern(productItem);

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
      <ListItemButton>
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

  const { isLoading, error, data: response, isSuccess } = useQuery([fetchSitesInfo], getSitesInfo);

  const [expanded, setExpanded] = React.useState("");

  const handleChange = (panel, row) => (event, newExpanded) => {
    setExpanded(newExpanded ? panel : false);
    setPattern("");
    onBeaconSelect("");
    onSiteSelect(row);
  };

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Sites
          </SuiTypography>
        </SuiBox>
      </SuiBox>
      <SuiBox pb={3}>
        {isSuccess &&
          response.map((item, index) => (
            <Accordion
              expanded={expanded === `panel_${index}`}
              onChange={handleChange(`panel_${index}`, item)}
              key={`site_${index}`}
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
