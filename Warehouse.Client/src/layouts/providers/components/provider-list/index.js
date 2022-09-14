import { useState } from "react";
import Card from "@mui/material/Card";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import Table from "examples/Tables/Table";
import { useQuery } from "react-query";
import { fetchProviders } from "utils/query-keys";
import { getProviders, deleteProvider } from "api/admin";
import Provider from "./provider";
import { ButtonGroup, Icon } from "@mui/material";
import SuiButton from "components/SuiButton";
import DeletePrompt from "components/DeletePrompt";
import useSecurity, { SecurityPermissions } from "services/security-provider";

const ProviderList = ({
  searchTerm = "",
  onEdit = () => {},
  onAdd = () => {},
  onDelete = () => {},
  reload,
}) => {
  const { hasPermissions: addPermissions } = useSecurity("PROVIDER", SecurityPermissions.Add);
  const { hasPermissions: editPermissions } = useSecurity("PROVIDER", SecurityPermissions.Edit);
  const { hasPermissions: deletePermissions } = useSecurity("PROVIDER", SecurityPermissions.Delete);
  const [page, setPage] = useState(1);
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchProviders, page, searchTerm, reload], getProviders);
  console.log("p_error", error);
  const handleDelete = async (item) => {
    try {
      await deleteProvider(item);
      return onDelete();
    } catch (err) {
      console.log("delete-provider", err);
    }
  };
  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6">Providers</SuiTypography>
        </SuiBox>
        {addPermissions && (
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
        {isSuccess && (
          <Table
            columns={[
              { name: "provider", align: "left" },
              { name: "description", align: "left" },
              { name: "culture", align: "left" },
              { name: "action", align: "center" },
            ]}
            rows={
              isSuccess &&
              response.map((item) => ({
                provider: <Provider name={item.name} alias={item.alias} />,
                description: (
                  <SuiTypography variant="caption" color="secondary" fontWeight="medium">
                    {item.description}
                  </SuiTypography>
                ),
                culture: (
                  <SuiTypography variant="caption" color="secondary" fontWeight="medium">
                    {item.culture}
                  </SuiTypography>
                ),
                action: (
                  <ButtonGroup variant="text" aria-label="text button group" color="text">
                    {editPermissions && (
                      <SuiButton variant="text" color="dark" onClick={() => onEdit(item)}>
                        <Icon>border_color</Icon>
                      </SuiButton>
                    )}
                    {deletePermissions && (
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
                    )}
                  </ButtonGroup>
                ),
              }))
            }
            page={page}
            totalPages={response.totalPages}
            onPageChange={(event, value) => setPage(value)}
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
};
export default ProviderList;
