import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiButton from "components/SuiButton";
import { ButtonGroup, Card, Icon } from "@mui/material";
import Table from "examples/Tables/Table";
import DeletePromt from "../delete-promt";
import routerIcon from "assets/images/internet-router.png";
import SuiAvatar from "components/SuiAvatar";
import * as auth from "services/auth-provider";
import { client } from "utils/api-client";
import React from "react";

const Locations = [
  "Unknown",
  "Center",
  "TopCenter",
  "TopLeft",
  "TopRight",
  "BottomCenter",
  "BottomLeft",
  "BottomRight",
  "CenterLeft",
  "CenterRight",
];

function Gauge({ data }) {
  return (
    <SuiBox display="flex" alignItems="baseline" flexDirection="column">
      <SuiTypography variant="caption" fontWeight="medium">
        {data.mac}
      </SuiTypography>
      <SuiTypography variant="caption" color="secondary">
        <SuiTypography variant="caption" color="success">
          Radius:
        </SuiTypography>
        &nbsp;&nbsp;{data.radius}
        dbm
      </SuiTypography>
      <SuiTypography variant="caption" color="secondary">
        <SuiTypography variant="caption" color="success">
          TX power:
        </SuiTypography>
        &nbsp;&nbsp;{data.txPower}dbm
      </SuiTypography>
    </SuiBox>
  );
}

export default function Gateways({
  data,
  onAdd = () => {},
  onEdit = () => {},
  onDelete = () => {},
}) {
  const handleDelete = async (item) => {
    const token = await auth.getToken();
    try {
      await client(`sites/${item.siteId}/delete-gw/${item.macAddress}`, { token });
      return onDelete();
    } catch (err) {
      console.log("delete-gw", err);
    }
  };

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6">Gateways</SuiTypography>
        </SuiBox>
        {data && (
          <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
            <SuiButton variant="gradient" color="primary" onClick={onAdd}>
              <Icon sx={{ fontWeight: "bold" }}>add</Icon>
              &nbsp;new
            </SuiButton>
          </SuiBox>
        )}
      </SuiBox>
      <SuiBox
        sx={{
          "& .MuiTableRow-root:not(:last-child)": {
            "& td": {
              borderBottom: ({ borders: { borderWidth, borderColor } }) =>
                `${borderWidth[1]} solid ${borderColor}`,
            },
          },
        }}
      >
        {data && data.gateways && (
          <Table
            columns={[
              { name: "mac", align: "left" },
              { name: "name", align: "center" },
              { name: "env. factor", align: "center" },
              { name: "location", align: "center" },
              { name: "gauge", align: "center" },
              { name: "", align: "center" },
            ]}
            rows={data.gateways.map((item) => ({
              mac: (
                <SuiBox display="flex" alignItems="center">
                  <SuiBox mx={2}>
                    <SuiAvatar src={routerIcon} alt={item.name} size="sm" variant="rounded" />
                  </SuiBox>
                  <SuiTypography variant="button" fontWeight="medium">
                    {item.macAddress}
                  </SuiTypography>
                </SuiBox>
              ),
              name: (
                <SuiTypography variant="caption" color="secondary">
                  {item.name}
                </SuiTypography>
              ),
              "env. factor": (
                <SuiTypography variant="caption" color="secondary">
                  {item.envFactor}
                </SuiTypography>
              ),
              location: (
                <SuiTypography variant="caption" color="secondary">
                  {Locations[item.location]}
                </SuiTypography>
              ),
              gauge: <Gauge data={item.gauge} />,
              "": (
                <ButtonGroup variant="text" aria-label="text button group" color="text">
                  <SuiButton
                    variant="text"
                    color="dark"
                    onClick={() => onEdit({ ...item, siteId: data.id })}
                  >
                    <Icon>border_color</Icon>
                  </SuiButton>
                  <DeletePromt
                    renderButton={(handleClickOpen) => (
                      <SuiButton
                        variant="text"
                        color="error"
                        onClick={(e) => {
                          e.stopPropagation();
                          handleClickOpen();
                          e.preventDefault();
                        }}
                      >
                        <Icon>delete</Icon>
                      </SuiButton>
                    )}
                    onDelete={() => handleDelete({ ...item, siteId: data.id })}
                  />
                </ButtonGroup>
              ),
            }))}
          />
        )}
      </SuiBox>
    </Card>
  );
}
