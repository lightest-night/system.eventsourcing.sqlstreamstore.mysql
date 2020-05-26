CREATE TABLE IF NOT EXISTS checkpoints
(
    id              BIGINT          NOT NULL    AUTO_INCREMENT,
    checkpoint_id   NVARCHAR(500)   NOT NULL,
    checkpoint      BIGINT          NULL,
    CONSTRAINT pk_checkpoints PRIMARY KEY (id),
    CONSTRAINT uq_checkpoints_checkpoint_id UNIQUE KEY (checkpoint_id)
) ENGINE = InnoDB
  CHARACTER SET utf8mb4
  COLLATE utf8mb4_unicode_ci;