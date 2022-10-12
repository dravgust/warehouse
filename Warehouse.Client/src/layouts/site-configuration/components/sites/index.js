import React, { useState } from "react";
import { ButtonGroup, Card, Icon } from "@mui/material";
import SuiBox from "components/SuiBox";
import SuiButton from "components/SuiButton";
import SuiTypography from "components/SuiTypography";
import Table from "examples/Tables/Table";
import DeletePrompt from "../delete-promt";
import * as auth from "services/auth-provider";
import { client } from "utils/api-client";
import { useQuery } from "react-query";
import SuiAvatar from "../../../../components/SuiAvatar";
import siteIcon from "../../../../assets/images/area-floor-size.png";
import { getSites } from "../../../../api/warehouse";
import { fetchSites } from "../../../../utils/query-keys";

export default function Sites({
  searchTerm = "",
  onSelect = () => {},
  selectedItem,
  onAdd = () => {},
  onEdit = () => {},
  onDelete = () => {},
  refresh,
}) {
  const [page, setPage] = useState(1);
  const { isLoading, isSuccess, data, error } = useQuery(
    [fetchSites, page, searchTerm, refresh],
    getSites
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

  React.useEffect(() => {
    isSuccess && data.items.length > 0 && onSelect(data.items[0], `row-${0}`);
  }, [isSuccess]);

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
                  <SuiTypography
                    variant="button"
                    fontWeight="medium"
                    style={{ overflow: "hidden", textOverflow: "ellipsis", width: "11rem" }}
                  >
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
                    <Icon>border_color</Icon>
                  </SuiButton>
                  <DeletePrompt
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
