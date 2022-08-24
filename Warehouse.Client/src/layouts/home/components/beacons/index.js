import { useState } from "react";
import Card from "@mui/material/Card";
import Icon from "@mui/material/Icon";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import Table from "examples/Tables/Table";
import { useQuery } from "react-query";
import { formatDistance } from "date-fns";
import beaconIcon from "assets/images/hotspot-tower.png";
import SensorsSharpIcon from "@mui/icons-material/SensorsSharp";
import { fetchAssets } from "../../../../utils/query-keys";
import { getAssets } from "../../../../services/warehouse-service";
import SuiInput from "../../../../components/SuiInput";

function Assets({ selectedItem, onRowSelect = () => {} }) {
  const [page, setPage] = useState(1);
  const [pattern, setPattern] = useState("");
  const onSearchBeacon = (beacon) => setPattern(beacon);
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchAssets, page, pattern], getAssets);
  function Beacon({ name, product }) {
    return (
      <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
        <SuiBox mr={2}>
          <SensorsSharpIcon fontSize="large" />
        </SuiBox>
        <SuiBox display="flex" flexDirection="column">
          <SuiTypography variant="button" fontWeight="medium">
            {name}
          </SuiTypography>
          <SuiTypography variant="caption" color="primary">
            {product ? product.name : "n/a"}
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    );
  }

  function Site({ timeStamp, site }) {
    return (
      <SuiBox display="flex" alignItems="center" px={1}>
        <SuiBox mr={2}></SuiBox>
        <SuiBox display="flex" flexDirection="column">
          <SuiTypography variant="h6" color="info" mt={-2}>
            {site ? site.name : "n/a"}
          </SuiTypography>
          <SuiTypography variant="caption" fontWeight="regular">
            {/*format(new Date(item.timeStamp), "dd/MM/yyy HH:mm:ss")*/}
            {formatDistance(new Date(timeStamp), new Date(), { addSuffix: true })}
          </SuiTypography>
        </SuiBox>
      </SuiBox>
    );
  }

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6" gutterBottom>
            Beacons
          </SuiTypography>
          <SuiBox display="flex" alignItems="center" lineHeight={0}>
            <Icon
              sx={{
                fontWeight: "bold",
                color: ({ palette: { info } }) => info.main,
                mt: -0.5,
              }}
            >
              done
            </Icon>
            <SuiTypography variant="button" fontWeight="regular" color="text">
              &nbsp;<strong>{response && response.totalItems}</strong> items
            </SuiTypography>
          </SuiBox>
        </SuiBox>
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
        <SuiBox px={2}>
          <SuiInput
            className="search-products"
            placeholder="Type here..."
            icon={{
              component: "search",
              direction: "left",
            }}
            onChange={(event) => onSearchBeacon(event.target.value)}
          />
        </SuiBox>
        {isSuccess && (
          <Table
            columns={[
              { name: "beacon", align: "left" },
              { name: "last location", align: "center" },
            ]}
            rows={
              isSuccess &&
              response.items.map((item) => ({
                key: item.macAddress,
                item: item,
                beacon: <Beacon image={beaconIcon} name={item.macAddress} product={item.product} />,
                "last location": <Site timeStamp={item.timeStamp} site={item.site}></Site>,
              }))
            }
            page={page}
            totalPages={response.totalPages}
            onPageChange={(event, value) => setPage(value)}
            onSelect={onRowSelect}
            selectedKey={selectedItem ? selectedItem.macAddress : ""}
          />
        )}
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

export default Assets;
