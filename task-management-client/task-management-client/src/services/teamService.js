import api from "../api/axios";

export const getTeams = async () => {
  const response = await api.get("/Teams");
  return response.data;
};

export const getTeamById = async (id) => {
  const response = await api.get(`/Teams/${id}`);
  return response.data;
};

export const createTeam = async (team) => {
  const response = await api.post(
    "/Teams",
    team
  );

  return response.data;
};

export const updateTeam = async (
  id,
  team
) => {
  const response = await api.put(
    `/Teams/${id}`,
    team
  );

  return response.data;
};

export const deleteTeam = async (id) => {
  const response = await api.delete(
    `/Teams/${id}`
  );

  return response.data;
};

export const addTeamMember = async (
  teamId,
  userId
) => {
  const response = await api.post(
    `/Teams/${teamId}/members/${userId}`
  );

  return response.data;
};

export const removeTeamMember = async (
  teamId,
  userId
) => {
  const response = await api.delete(
    `/Teams/${teamId}/members/${userId}`
  );

  return response.data;
};