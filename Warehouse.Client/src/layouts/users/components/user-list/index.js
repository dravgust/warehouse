import { useState } from "react";
import Card from "@mui/material/Card";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiBadge from "components/SuiBadge";
import Table from "examples/Tables/Table";
import { format } from "date-fns";
import { he } from "date-fns/locale";
import { ProviderName } from "data/providers";
import { useQuery } from "react-query";
import { fetchUsers } from "utils/query-keys";
import { getUsers } from "api/admin";
import User from "./user";
import Function from "./function";
import { ButtonGroup, Icon } from "@mui/material";
import SuiButton from "components/SuiButton";
import { LogLevel } from "data/log-level";
import DeletePrompt from "../../../site-configuration/components/delete-promt";
import { deleteUser } from "api/admin";

const UserList = ({
  searchTerm = "",
  onEdit = () => {},
  onAdd = () => {},
  onDelete = () => {},
  reload,
}) => {
  const [page, setPage] = useState(1);
  const {
    isLoading,
    error,
    data: response,
    isSuccess,
  } = useQuery([fetchUsers, page, searchTerm, reload], getUsers);
  const handleDelete = async (item) => {
    try {
      await deleteUser(item);
      return onDelete();
    } catch (err) {
      console.log("delete-user", err);
    }
  };
  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiBox>
          <SuiTypography variant="h6">Users</SuiTypography>
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
              { name: "user", align: "left" },
              { name: "function", align: "left" },
              { name: "log level", align: "center" },
              { name: "registered", align: "center" },
              { name: "action", align: "center" },
            ]}
            rows={
              isSuccess &&
              response.items.map((item) => ({
                user: <User name={item.username} email={item.email} />,
                function: <Function job={item.type} org={ProviderName(item.providerId)} />,
                "log level": (
                  <SuiBadge
                    variant="gradient"
                    badgeContent={LogLevel[item.logLevel]}
                    color={item.logLevel === 3 || item.logLevel === 4 ? "error" : "light"}
                    size="xs"
                    container
                  />
                ),
                registered: (
                  <SuiTypography variant="caption" color="secondary" fontWeight="medium">
                    {item.registered
                      ? format(new Date(item.registered), "HH:mm:ss, dd MMM", { locale: he })
                      : "n/a"}
                  </SuiTypography>
                ),
                action: (
                  <ButtonGroup variant="text" aria-label="text button group" color="text">
                    <SuiButton variant="text" color="dark" onClick={() => onEdit(item)}>
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
export default UserList;
