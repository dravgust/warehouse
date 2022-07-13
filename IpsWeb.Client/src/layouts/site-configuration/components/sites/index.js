import { useState } from "react";
import { Button, ButtonGroup, Card, Icon } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiButton from "components/SuiButton";
import SuiTypography from "components/SuiTypography";
import Table from "examples/Tables/Table";
import DeletePromt from "../delete-promt";

import * as auth from "auth-provider";
import { client } from "utils/api-client";
import { useQuery } from "react-query";
import SuiAvatar from "../../../../components/SuiAvatar";
import siteIcon from "../../../../assets/images/area-floor-size.png";

export default function Sites({
  onSelect = () => {},
  selectedItem,
  onAdd = () => {},
  onEdit = () => {},
  onDelete = () => {},
  refresh,
}) {
  const [page, setPage] = useState(1);

  async function fetchSites(page) {
    const token = await auth.getToken();
    const res = await client(`sites?page=${page}&size=10&searchTerm=`, { token });
    return res;
  }
  const { isLoading, isSuccess, data, error } = useQuery(
    ["site-list", page, refresh],
    () => fetchSites(page),
    {
      keepPreviousData: false,
      refetchOnWindowFocus: false,
    }
  );

  const handleDelete = async (item) => {
    const token = await auth.getToken();
    try {
      await client(`sites/${item.id}/delete`, { token });
      return onDelete();
    } catch (err) {
      console.log("delete-item", err);
    }
  };

  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6">Sites</SuiTypography>
        </SuiBox>
        <SuiBox display="flex" alignItems="center" mt={{ xs: 2, sm: 0 }} ml={{ xs: -1.5, sm: 0 }}>
          <SuiButton variant="gradient" color="primary" onClick={onAdd}>
            <Icon sx={{ fontWeight: "bold" }}>add</Icon>
            &nbsp;new
          </SuiButton>
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
        {isSuccess && (
          <Table
            columns={[
              { name: "name", align: "left" },
              { name: "top length", align: "center" },
              { name: "left length", align: "center" },
              { name: "error", align: "center" },
              { name: "", align: "center" },
            ]}
            rows={data.items.map((item) => ({
              item,
              name: (
                <SuiBox display="flex" alignItems="center">
                  <SuiBox mx={2}>
                    <SuiAvatar src={siteIcon} alt={item.name} size="sm" variant="rounded" />
                  </SuiBox>
                  <SuiTypography variant="button" fontWeight="medium">
                    {item.name}
                  </SuiTypography>
                </SuiBox>
              ),
              "top length": (
                <SuiTypography variant="caption" color="secondary">
                  {item.topLength}
                </SuiTypography>
              ),
              "left length": (
                <SuiTypography variant="caption" color="secondary">
                  {item.leftLength}
                </SuiTypography>
              ),
              error: (
                <SuiTypography variant="caption" color="secondary">
                  {item.error}
                </SuiTypography>
              ),
              "": (
                <ButtonGroup variant="text" aria-label="text button group" color="text">
                  <SuiButton variant="text" color="dark" onClick={onEdit}>
                    <Icon>edit</Icon>
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
                    onDelete={() => handleDelete(item)}
                  />
                </ButtonGroup>
              ),
            }))}
            page={page}
            totalPages={data.totalPages}
            onPageChange={(event, value) => setPage(value)}
            onSelect={onSelect}
            selectedKey={selectedItem ? selectedItem.key : ""}
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
