CREATE TABLE [dbo].[state_licensure] (
    [stl_key]     UNIQUEIDENTIFIER NOT NULL,
    [stl_usr_key] UNIQUEIDENTIFIER NOT NULL,
    [stl_sta_key] TINYINT          NOT NULL,
    CONSTRAINT [PK_State_Licensure] PRIMARY KEY CLUSTERED ([stl_key] ASC),
    CONSTRAINT [FK__state_lic__stl_s__2FAFEA50] FOREIGN KEY ([stl_sta_key]) REFERENCES [dbo].[states] ([sta_key]),
    CONSTRAINT [FK_User] FOREIGN KEY ([stl_usr_key]) REFERENCES [dbo].[users] ([usr_key])
);


GO
CREATE NONCLUSTERED INDEX [_dta_index_state_licensure_35_877298235__K3_K2_1]
    ON [dbo].[state_licensure]([stl_sta_key] ASC, [stl_usr_key] ASC)
    INCLUDE([stl_key]);

