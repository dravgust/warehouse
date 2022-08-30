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
import SuiButton from "../../../../components/SuiButton";

const UserList = ({ onEdit = () => {} }) => {
  const [page, setPage] = useState(1);
  const { isLoading, error, data: response, isSuccess } = useQuery([fetchUsers, page], getUsers);
  return (
    <Card>
      <SuiBox display="flex" justifyContent="space-between" alignItems="center" p={3}>
        <SuiTypography variant="h6">Users</SuiTypography>
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
              { name: "status", align: "center" },
              { name: "registered", align: "center" },
              { name: "action", align: "center" },
            ]}
            rows={
              isSuccess &&
              response.items.map((item) => ({
                user: <User name={item.username} email={item.email} />,
                function: <Function job={item.type} org={ProviderName(item.providerId)} />,
                status: (
                  <SuiBadge
                    variant="gradient"
                    badgeContent="n/a"
                    color="dark"
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
                      <Icon>edit</Icon>
                    </SuiButton>
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
