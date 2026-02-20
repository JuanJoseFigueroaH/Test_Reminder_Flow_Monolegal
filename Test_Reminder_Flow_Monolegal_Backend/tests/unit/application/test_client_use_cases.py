import pytest
from unittest.mock import AsyncMock

from src.application.use_cases.client_use_cases import ClientUseCases
from src.application.commands.create_client_command import CreateClientCommand
from src.domain.entities.client import Client
from src.domain.exceptions.domain_exceptions import ClientNotFoundException


class TestClientUseCases:
    @pytest.fixture
    def use_cases(self, mock_client_repository, mock_cache):
        return ClientUseCases(
            client_repository=mock_client_repository,
            cache=mock_cache
        )

    @pytest.mark.asyncio
    async def test_get_all_clients_from_cache(self, use_cases, mock_cache, mock_client):
        mock_cache.get.return_value = [mock_client.to_dict()]
        
        result = await use_cases.get_all_clients()
        
        assert len(result) == 1
        assert result[0].name == mock_client.name
        mock_cache.get.assert_called_once()

    @pytest.mark.asyncio
    async def test_get_all_clients_from_repository(self, use_cases, mock_client_repository, mock_cache, mock_client):
        mock_cache.get.return_value = None
        mock_client_repository.get_all.return_value = [mock_client]
        
        result = await use_cases.get_all_clients()
        
        assert len(result) == 1
        mock_client_repository.get_all.assert_called_once()
        mock_cache.set.assert_called_once()

    @pytest.mark.asyncio
    async def test_get_client_by_id_success(self, use_cases, mock_client_repository, mock_cache, mock_client):
        mock_cache.get.return_value = None
        mock_client_repository.get_by_id.return_value = mock_client
        
        result = await use_cases.get_client_by_id(mock_client.id)
        
        assert result.id == mock_client.id
        mock_client_repository.get_by_id.assert_called_once_with(mock_client.id)

    @pytest.mark.asyncio
    async def test_get_client_by_id_not_found(self, use_cases, mock_client_repository, mock_cache):
        mock_cache.get.return_value = None
        mock_client_repository.get_by_id.return_value = None
        
        with pytest.raises(ClientNotFoundException):
            await use_cases.get_client_by_id("nonexistent_id")

    @pytest.mark.asyncio
    async def test_create_client(self, use_cases, mock_client_repository, mock_cache):
        command = CreateClientCommand(
            name="New Client",
            email="new@email.com",
            phone="+57 300 999 8888"
        )
        
        created_client = Client(
            id="new_id",
            name="New Client",
            email="new@email.com",
            phone="+57 300 999 8888"
        )
        
        mock_client_repository.create.return_value = created_client
        
        result = await use_cases.create_client(command)
        
        assert result.name == "New Client"
        mock_client_repository.create.assert_called_once()
        mock_cache.clear_pattern.assert_called()

    @pytest.mark.asyncio
    async def test_update_client_success(self, use_cases, mock_client_repository, mock_cache, mock_client):
        mock_client_repository.get_by_id.return_value = mock_client
        
        updated_client = Client(
            id=mock_client.id,
            name="Updated Name",
            email="updated@email.com",
            phone="+57 300 111 2222"
        )
        mock_client_repository.update.return_value = updated_client
        
        result = await use_cases.update_client(
            client_id=mock_client.id,
            name="Updated Name",
            email="updated@email.com",
            phone="+57 300 111 2222"
        )
        
        assert result.name == "Updated Name"
        mock_cache.clear_pattern.assert_called()

    @pytest.mark.asyncio
    async def test_delete_client_success(self, use_cases, mock_client_repository, mock_cache):
        mock_client_repository.delete.return_value = True
        
        result = await use_cases.delete_client("client_id")
        
        assert result is True
        mock_client_repository.delete.assert_called_once_with("client_id")
        mock_cache.clear_pattern.assert_called()
