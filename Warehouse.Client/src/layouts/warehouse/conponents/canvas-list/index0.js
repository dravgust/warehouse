import {
  Accordion,
  AccordionDetails,
  AccordionSummary,
} from "../../../home/components/sites/components/accordion";
import SuiBox from "components/SuiBox";
import Site from "../../../home/components/sites/components/site";
import SuiInput from "components/SuiInput";
import { FixedSizeList } from "react-window";
import SuiTypography from "components/SuiTypography";
import * as React from "react";
import { useEffect, useState } from "react";
import { useSoftUIController } from "../../../../context";
import { useQuery } from "react-query";
import { fetchSitesInfo } from "../../../../utils/query-keys";
import { getSitesInfo } from "../../../../services/warehouse-service";
import ListItem from "@mui/material/ListItem";
import ListItemButton from "@mui/material/ListItemButton";
import ListItemIcon from "@mui/material/ListItemIcon";
import QrCode2SharpIcon from "@mui/icons-material/QrCode2Sharp";
import ListItemText from "@mui/material/ListItemText";
import TabOutlinedIcon from "@mui/icons-material/TabOutlined";

const CanvasList = () => {
  const [pattern, setPattern] = useState("");
  const [controller] = useSoftUIController();
  const [expanded, setExpanded] = React.useState("");

  const { direction } = controller;
  const handleChange = (panel, row) => (event, newExpanded) => {
    setExpanded(newExpanded ? panel : false);
  };
  const onSearch = (productItem) => setPattern(productItem);
  const { isLoading, error, data: response, isSuccess } = useQuery([fetchSitesInfo], getSitesInfo);
  console.log("canvas-list", response);
  let assets = [];

  const Row = ({ index, style }) => (
    <ListItem
      key={`b_${index}`}
      style={style}
      component="div"
      disablePadding
      onClick={() => {}}
      sx={{
        borderBottom: ({ borders: { borderWidth, borderColor } }) =>
          `${borderWidth[1]} solid ${borderColor}`,
      }}
      selected={false}
      //secondaryAction={ }
    >
      <ListItemButton dir={direction}>
        <ListItemIcon>
          <QrCode2SharpIcon fontSize="large" />
        </ListItemIcon>
        <ListItemText
          primaryTypographyProps={{ color: "#cb0c9f", fontSize: "0.875rem" }}
          primary={assets[index].name}
          secondaryTypographyProps={{ color: "#82d616", fontSize: "0.75rem" }}
          secondary={`${assets[index].beacons.length} items`}
        />
      </ListItemButton>
    </ListItem>
  );
  return (
    <SuiBox pb={3}>
      {isSuccess &&
        response[0].products.map((item, index) => (
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
                <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
                  <SuiBox mr={2}>
                    <TabOutlinedIcon fontSize="large" />
                  </SuiBox>
                  <SuiBox display="flex" flexDirection="column">
                    <SuiTypography variant="button" fontWeight="medium" color={"info"}>
                      {item.name}
                    </SuiTypography>
                    <SuiTypography variant="caption" color="success">
                      {item.beacons.length}&nbsp;beacons
                    </SuiTypography>
                  </SuiBox>
                </SuiBox>
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
                  onChange={(event) => onSearch(event.target.value)}
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
  );
};

export default CanvasList;
