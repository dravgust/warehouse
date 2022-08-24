import { useState } from "react";
import Card from "@mui/material/Card";
import SuiBox from "components/SuiBox";
import SuiTypography from "components/SuiTypography";
import SuiBadge from "components/SuiBadge";
import DashboardLayout from "examples/LayoutContainers/DashboardLayout";
import DashboardNavbar from "examples/Navbars/DashboardNavbar";
import Footer from "examples/Footer";
import Table from "examples/Tables/Table";
import { useQuery } from "react-query";
import { format } from "date-fns";
import { he } from "date-fns/locale";
import { getUsers } from "../../services/administration-service";
import { fetchUsers } from "../../utils/query-keys";
import PersonIcon from "@mui/icons-material/Person";
import { ProviderName } from "../../utils/providers";

function User({ name, email }) {
  return (
    <SuiBox display="flex" alignItems="center" px={1} py={0.5}>
      <SuiBox mr={2}>
        <PersonIcon fontSize="large" />
      </SuiBox>
      <SuiBox display="flex" flexDirection="column">
        <SuiTypography variant="button" fontWeight="medium">
          {name || "n/a"}
        </SuiTypography>
        <SuiTypography variant="caption" color="secondary">
          {email || "@"}
        </SuiTypography>
      </SuiBox>
    </SuiBox>
  );
}

function Function({ job, org }) {
  return (
    <SuiBox display="flex" flexDirection="column">
      <SuiTypography variant="caption" fontWeight="medium" color="text">
        {job}
      </SuiTypography>
      <SuiTypography variant="caption" color="secondary">
        {org}
      </SuiTypography>
    </SuiBox>
  );
}

function Users() {
  const [page, setPage] = useState(1);
  const { isLoading, error, data: response, isSuccess } = useQuery([fetchUsers, page], getUsers);

  return (
    <DashboardLayout>
      <DashboardNavbar />
      <SuiBox py={3}>
        <SuiBox mb={3}>
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
                        <SuiTypography
                          component="a"
                          href="#"
                          variant="caption"
                          color="secondary"
                          fontWeight="medium"
                        >
                          ...
                        </SuiTypography>
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
        </SuiBox>
      </SuiBox>
      <Footer />
    </DashboardLayout>
  );
}

export default Users;
